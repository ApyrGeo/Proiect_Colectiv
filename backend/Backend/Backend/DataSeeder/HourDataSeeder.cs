using Backend.Context;
using Backend.Domain;
using Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataSeeder;

public class HourDataSeeder(AcademicAppContext context)
{
    private readonly AcademicAppContext _context = context;

    public async Task SeedAsync()
    {
        if (await _context.Hours.AnyAsync())
            return;

        var subjects = await _context.Subjects
            .Include(s => s.GroupYear)
                .ThenInclude(gy => gy.Specialisation)
                    .ThenInclude(sp => sp.Faculty)
            .Include(s => s.GroupYear)
                .ThenInclude(gy => gy.StudentGroups)
                    .ThenInclude(g => g.StudentSubGroups)
            .ToListAsync();

        var teachers = await _context.Teachers
            .Include(t => t.Faculty)
            .ToListAsync();

        var classrooms = await _context.Classrooms.ToListAsync();

        if (!subjects.Any() || !teachers.Any() || !classrooms.Any())
            return;

        var rnd = new Random();
        var dayValues = Enum.GetValues<HourDay>().Cast<HourDay>().ToArray();
        var intervals = new[] { "8-10", "10-12", "12-14", "14-16", "16-18", "18-20" };

        // Track occupancy and counts
        var occupiedTeacherSlots = new HashSet<string>();
        var occupiedClassroomSlots = new HashSet<string>();
        var occupiedGroupSlots = new HashSet<string>(); // keys for Group/GroupYear/SubGroup

        // per-classroom counters: day -> count, weekly total
        var classroomDayCount = classrooms.ToDictionary(
            c => c.Id,
            c => dayValues.ToDictionary(d => d, d => 0)
        );
        var classroomWeeklyCount = classrooms.ToDictionary(c => c.Id, c => 0);

        var hoursToAdd = new List<Hour>();

        static string TeacherKey(int id, HourDay day, string interval) => $"T:{id}:{day}:{interval}";
        static string ClassroomKey(int id, HourDay day, string interval) => $"C:{id}:{day}:{interval}";
        static string GroupKey(string prefix, int id, HourDay day, string interval) => $"{prefix}:{id}:{day}:{interval}";

        bool TryFindSlotAndClassroom(Func<HourDay, string, bool> slotOk, out HourDay foundDay, out string foundInterval, out Classroom selectedClassroom)
        {
            // random attempts first
            for (int attempt = 0; attempt < 100; attempt++)
            {
                var day = dayValues[rnd.Next(dayValues.Length)];
                var interval = intervals[rnd.Next(intervals.Length)];

                if (!slotOk(day, interval))
                    continue;

                // pick a classroom that is free at that slot and respects per-day and per-week limits
                var classroom = classrooms.FirstOrDefault(c =>
                    !occupiedClassroomSlots.Contains(ClassroomKey(c.Id, day, interval))
                    && classroomDayCount[c.Id][day] < intervals.Length // per-day slots (6)
                    && classroomWeeklyCount[c.Id] < 30 // weekly max per classroom
                );

                if (classroom == null)
                    continue;

                foundDay = day;
                foundInterval = interval;
                selectedClassroom = classroom;
                // update counts here to reserve the spot
                classroomDayCount[classroom.Id][day]++;
                classroomWeeklyCount[classroom.Id]++;
                return true;
            }

            // fallback linear search
            foreach (var day in dayValues)
            {
                foreach (var interval in intervals)
                {
                    if (!slotOk(day, interval))
                        continue;

                    var classroom = classrooms.FirstOrDefault(c =>
                        !occupiedClassroomSlots.Contains(ClassroomKey(c.Id, day, interval))
                        && classroomDayCount[c.Id][day] < intervals.Length
                        && classroomWeeklyCount[c.Id] < 30
                    );

                    if (classroom == null) continue;

                    foundDay = day;
                    foundInterval = interval;
                    selectedClassroom = classroom;
                    classroomDayCount[classroom.Id][day]++;
                    classroomWeeklyCount[classroom.Id]++;
                    return true;
                }
            }

            foundDay = default;
            foundInterval = null!;
            selectedClassroom = null!;
            return false;
        }

        foreach (var subject in subjects)
        {
            var gy = subject.GroupYear;
            if (gy == null) continue;

            // find teachers matching faculty
            var facultyId = gy.Specialisation?.Faculty?.Id ?? 0;
            var candidateTeachers = teachers.Where(t => t.FacultyId == facultyId).ToList();
            if (!candidateTeachers.Any())
                candidateTeachers = teachers; // fallback

            // 1) Course: one hour referencing GroupYear
            {
                var teacher = candidateTeachers[rnd.Next(candidateTeachers.Count)];
                if (TryFindSlotAndClassroom((d, it) =>
                    !occupiedTeacherSlots.Contains(TeacherKey(teacher.Id, d, it))
                    // avoid group conflict for GY slot (GroupYear reference) — using a GY key
                    && !occupiedGroupSlots.Contains(GroupKey("GY", gy.Id, d, it))
                , out var day, out var interval, out var classroom))
                {
                    var h = new Hour
                    {
                        Day = day,
                        HourInterval = interval,
                        Frequency = HourFrequency.Weekly,
                        Subject = subject,
                        SubjectId = subject.Id,
                        Classroom = classroom,
                        ClassroomId = classroom.Id,
                        Teacher = teacher,
                        TeacherId = teacher.Id,
                        GroupYear = gy,
                        GroupYearId = gy.Id,
                        Category = HourCategory.Lecture
                    };

                    hoursToAdd.Add(h);
                    occupiedTeacherSlots.Add(TeacherKey(teacher.Id, day, interval));
                    occupiedClassroomSlots.Add(ClassroomKey(classroom.Id, day, interval));
                    occupiedGroupSlots.Add(GroupKey("GY", gy.Id, day, interval));
                }
            }

            // 2) For each StudentGroup -> seminar (75% weekly; else 50/50 First/Second)
            if (gy.StudentGroups != null)
            {
                foreach (var group in gy.StudentGroups)
                {
                    var p = rnd.NextDouble();
                    HourFrequency freq = p < 0.75 ? HourFrequency.Weekly
                        : (rnd.NextDouble() < 0.5 ? HourFrequency.FirstWeek : HourFrequency.SecondWeek);

                    var teacher = candidateTeachers[rnd.Next(candidateTeachers.Count)];

                    if (TryFindSlotAndClassroom((d, it) =>
                        !occupiedGroupSlots.Contains(GroupKey("G", group.Id, d, it))
                        && !occupiedTeacherSlots.Contains(TeacherKey(teacher.Id, d, it))
                    , out var day, out var interval, out var classroom))
                    {
                        var h = new Hour
                        {
                            Day = day,
                            HourInterval = interval,
                            Frequency = freq,
                            Subject = subject,
                            SubjectId = subject.Id,
                            Classroom = classroom,
                            ClassroomId = classroom.Id,
                            Teacher = teacher,
                            TeacherId = teacher.Id,
                            StudentGroup = group,
                            StudentGroupId = group.Id,
                            Category = HourCategory.Seminar
                        };

                        hoursToAdd.Add(h);
                        occupiedGroupSlots.Add(GroupKey("G", group.Id, day, interval));
                        occupiedTeacherSlots.Add(TeacherKey(teacher.Id, day, interval));
                        occupiedClassroomSlots.Add(ClassroomKey(classroom.Id, day, interval));
                    }
                }

                // 3) For each StudentSubGroup -> lab (50% weekly; else 25/25 first/second)
                foreach (var group in gy.StudentGroups)
                {
                    if (group.StudentSubGroups == null) continue;
                    foreach (var sub in group.StudentSubGroups)
                    {
                        var r = rnd.NextDouble();
                        HourFrequency freq = r < 0.5 ? HourFrequency.Weekly
                            : (rnd.NextDouble() < 0.5 ? HourFrequency.FirstWeek : HourFrequency.SecondWeek);

                        var teacher = candidateTeachers[rnd.Next(candidateTeachers.Count)];

                        if (TryFindSlotAndClassroom((d, it) =>
                            !occupiedGroupSlots.Contains(GroupKey("SS", sub.Id, d, it))
                            && !occupiedTeacherSlots.Contains(TeacherKey(teacher.Id, d, it))
                        , out var day, out var interval, out var classroom))
                        {
                            var h = new Hour
                            {
                                Day = day,
                                HourInterval = interval,
                                Frequency = freq,
                                Subject = subject,
                                SubjectId = subject.Id,
                                Classroom = classroom,
                                ClassroomId = classroom.Id,
                                Teacher = teacher,
                                TeacherId = teacher.Id,
                                StudentSubGroup = sub,
                                StudentSubGroupId = sub.Id,
                                Category = HourCategory.Laboratory
                            };

                            hoursToAdd.Add(h);
                            occupiedGroupSlots.Add(GroupKey("SS", sub.Id, day, interval));
                            occupiedTeacherSlots.Add(TeacherKey(teacher.Id, day, interval));
                            occupiedClassroomSlots.Add(ClassroomKey(classroom.Id, day, interval));
                        }
                    }
                }
            }
        }

        if (hoursToAdd.Any())
        {
            await _context.Hours.AddRangeAsync(hoursToAdd);
            await _context.SaveChangesAsync();
        }
    }
}
