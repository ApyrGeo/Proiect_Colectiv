using Microsoft.EntityFrameworkCore;
using TrackForUBB.Repository.Context;
using TrackForUBB.Repository.EFEntities;

namespace TrackForUBB.Repository.DataSeeder;

public class ExamDataSeeder(AcademicAppContext context)
{
    private readonly AcademicAppContext _context = context;
    private static readonly Random _random = new Random(6767);

    public async Task SeedAsync()
    {
        await AssignHolderTeachersAsync();

        if (await _context.ExamEntries.AnyAsync())
            return;


        await SeedExamEntriesAsync();
    }

    private async Task AssignHolderTeachersAsync()
    {
        var subjectsWithoutHolder = await _context.Subjects
            .Where(s => s.HolderTeacher == null)
            .Include(s => s.Contracts)
                .ThenInclude(c => c.Semester)
                    .ThenInclude(sem => sem.PromotionYear)
                        .ThenInclude(py => py.Promotion)
                            .ThenInclude(p => p.Specialisation)
                                .ThenInclude(sp => sp.Faculty)
            .ToListAsync();

        if (subjectsWithoutHolder.Count == 0)
            return;

        // Get teachers that currently have no held subjects
        var teachersWithoutSubject = await _context.Teachers
            .Include(t => t.Faculty)
            .Include(t => t.HeldSubjects)
            .Where(t => !t.HeldSubjects.Any())
            .ToListAsync();

        if (teachersWithoutSubject.Count == 0)
            return;

        foreach (var subject in subjectsWithoutHolder)
        {
            // Determine faculty from subject's first contract (if available)
            var contract = subject.Contracts?.FirstOrDefault();
            var facultyId = contract?.Semester?.PromotionYear?.Promotion?.Specialisation?.Faculty?.Id;

            // Prefer a teacher from the same faculty, fallback to any available teacher
            var availableTeacher = teachersWithoutSubject.FirstOrDefault(t => t.FacultyId == facultyId)
                                    ?? teachersWithoutSubject.FirstOrDefault();

            if (availableTeacher == null)
                break;

            // Add subject to teacher's held subjects
            availableTeacher.HeldSubjects.Add(subject);

            // Set both sides of the relationship for consistency
            subject.HolderTeacher = availableTeacher;

            // Remove teacher from pool so each teacher gets only ONE subject
            teachersWithoutSubject.Remove(availableTeacher);
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedExamEntriesAsync()
    {
        var subjects = await _context.Subjects
            .Where(s => s.HolderTeacher != null)
            .ToListAsync();

        var studentGroups = await _context.Groups.ToListAsync();
        var classrooms = await _context.Classrooms.ToListAsync();

        if (subjects.Count == 0 || studentGroups.Count == 0 || classrooms.Count == 0)
            return;

        var examEntries = new List<ExamEntry>();
        var occupiedSlots = new Dictionary<(int ClassroomId, DateTime Date), DateTime>();
        var examStartDate = DateTime.SpecifyKind(new DateTime(2025, 1, 15), DateTimeKind.Utc);
        var examEndDate = DateTime.SpecifyKind(new DateTime(2025, 2, 15), DateTimeKind.Utc);

        // Step 1: For each subject, 50% chance to create exam entries
        foreach (var subject in subjects)
        {
            // 50% chance this subject has exams
            if (_random.NextDouble() > 0.5)
                continue; // Skip this subject entirely

            // Step 2: For this subject, create entries for multiple groups (5-15)
            var minGroups = Math.Min(5, studentGroups.Count);
            var maxGroups = Math.Min(15, studentGroups.Count);
            var groupCount = _random.Next(minGroups, maxGroups + 1);
            var selectedGroups = studentGroups.OrderBy(_ => _random.Next()).Take(groupCount).ToList();

            foreach (var group in selectedGroups)
            {
                // Create entry (initially unscheduled with null fields)
                var examEntry = new ExamEntry
                {
                    ExamDate = null,
                    Duration = null,
                    ClassroomId = null,
                    SubjectId = subject.Id,
                    StudentGroupId = group.Id,
                    Classroom = null,
                    Subject = subject,
                    StudentGroup = group
                };

                examEntries.Add(examEntry);

                // Step 3: 50% chance to schedule this specific group's exam
                if (_random.NextDouble() > 0.5)
                    continue; // Leave as unscheduled (null fields)

                // Try to find a valid exam slot
                var examScheduled = false;
                var attempts = 0;
                const int maxAttempts = 100;

                while (!examScheduled && attempts < maxAttempts)
                {
                    attempts++;

                    // Random date in exam period
                    var daysInPeriod = (examEndDate - examStartDate).Days;
                    var randomDay = examStartDate.AddDays(_random.Next(daysInPeriod));

                    // Random time between 8:00 and 16:00
                    var hour = _random.Next(8, 17);
                    var examDate = DateTime.SpecifyKind(
                        new DateTime(randomDay.Year, randomDay.Month, randomDay.Day, hour, 0, 0),
                        DateTimeKind.Utc
                    );

                    // Duration: 1, 2, or 3 hours (in minutes)
                    var durationHours = _random.Next(1, 4);
                    var duration = durationHours * 60;
                    var examEndTime = examDate.AddMinutes(duration);

                    // Select random classroom
                    var classroom = classrooms[_random.Next(classrooms.Count)];

                    // Check for conflicts in this classroom
                    var hasConflict = occupiedSlots
                        .Where(slot => slot.Key.ClassroomId == classroom.Id)
                        .Any(slot =>
                        {
                            var existingStart = slot.Key.Date;
                            var existingEnd = slot.Value;
                            return examDate < existingEnd && examEndTime > existingStart;
                        });

                    if (hasConflict)
                        continue;

                    // Schedule the exam by filling in the fields
                    examEntry.ExamDate = examDate;
                    examEntry.Duration = duration;
                    examEntry.ClassroomId = classroom.Id;
                    examEntry.Classroom = classroom;

                    occupiedSlots[(classroom.Id, examDate)] = examEndTime;
                    examScheduled = true;
                }
            }
        }

        if (examEntries.Count > 0)
        {
            await _context.ExamEntries.AddRangeAsync(examEntries);
            await _context.SaveChangesAsync();
        }
    }
}