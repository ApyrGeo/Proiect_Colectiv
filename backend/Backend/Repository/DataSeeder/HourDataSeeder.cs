using TrackForUBB.Repository.Context;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Repository.DataSeeder;

public class HourDataSeeder(AcademicAppContext context)
{
	private readonly AcademicAppContext _context = context;
	private static readonly Random _random = new Random(6767);

	public async Task SeedAsync()
	{
		if (await _context.Hours.AnyAsync())
			return;

		var promotions = await _context.Promotions
			.Include(p => p.Specialisation)
				.ThenInclude(s => s.Faculty)
			.Include(p => p.StudentGroups)
				.ThenInclude(sg => sg.StudentSubGroups)
			.Include(p => p.Years)
				.ThenInclude(y => y.PromotionSemesters)
			.ToListAsync();

		var subjects = await _context.Subjects.ToListAsync();
		var teachers = await _context.Teachers
			.Include(t => t.Faculty)
			.ToListAsync();
		var classrooms = await _context.Classrooms.ToListAsync();

		if (promotions.Count == 0 || subjects.Count == 0 || teachers.Count == 0 || classrooms.Count == 0)
			return;

		var dayValues = Enum.GetValues<HourDay>().Cast<HourDay>()
			.Where(d => d != HourDay.Unknown && d != HourDay.Saturday && d != HourDay.Sunday)
			.ToArray();

		var intervals = new[] { "08-10", "10-12", "12-14", "14-16", "16-18", "18-20" };

		var occupiedTeacherSlots = new HashSet<string>();
		var occupiedClassroomSlots = new HashSet<string>();
		var occupiedSubGroupSlots = new HashSet<string>();

		var classroomDayCount = classrooms.ToDictionary(
			c => c.Id,
			c => dayValues.ToDictionary(d => d, d => 0)
		);
		var classroomWeeklyCount = classrooms.ToDictionary(c => c.Id, c => 0);

		var hoursToAdd = new List<Hour>();

		static string TeacherKey(int id, HourDay day, string interval) => $"T:{id}:{day}:{interval}";
		static string ClassroomKey(int id, HourDay day, string interval) => $"C:{id}:{day}:{interval}";
		static string SubGroupKey(int subId, HourDay day, string interval) => $"SG:{subId}:{day}:{interval}";

		bool TryFindSlotAndClassroom(Func<HourDay, string, bool> slotOk, out HourDay foundDay, out string foundInterval, out Classroom selectedClassroom)
		{
			for (int attempt = 0; attempt < 50; attempt++)
			{
				var day = dayValues[_random.Next(dayValues.Length)];
				var interval = intervals[_random.Next(intervals.Length)];

				if (!slotOk(day, interval))
					continue;

				var classroom = classrooms.FirstOrDefault(c =>
					!occupiedClassroomSlots.Contains(ClassroomKey(c.Id, day, interval))
					&& classroomDayCount[c.Id][day] < intervals.Length
					&& classroomWeeklyCount[c.Id] < 30
				);

				if (classroom == null)
					continue;

				foundDay = day;
				foundInterval = interval;
				selectedClassroom = classroom;

				classroomDayCount[classroom.Id][day]++;
				classroomWeeklyCount[classroom.Id]++;
				return true;
			}

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

		bool AnySubGroupOccupiedForPromotion(Promotion promotion, HourDay day, string interval)
		{
			if (promotion?.StudentGroups == null) return false;
			foreach (var g in promotion.StudentGroups)
			{
				if (g?.StudentSubGroups == null) continue;
				foreach (var ss in g.StudentSubGroups)
				{
					if (occupiedSubGroupSlots.Contains(SubGroupKey(ss.Id, day, interval)))
						return true;
				}
			}
			return false;
		}

		bool AnySubGroupOccupiedForGroup(StudentGroup group, HourDay day, string interval)
		{
			if (group?.StudentSubGroups == null) return false;
			foreach (var ss in group.StudentSubGroups)
			{
				if (occupiedSubGroupSlots.Contains(SubGroupKey(ss.Id, day, interval)))
					return true;
			}
			return false;
		}

		void ReserveSubGroupsForPromotion(Promotion promotion, HourDay day, string interval)
		{
			if (promotion?.StudentGroups == null) return;
			foreach (var g in promotion.StudentGroups)
			{
				if (g?.StudentSubGroups == null) continue;
				foreach (var ss in g.StudentSubGroups)
				{
					occupiedSubGroupSlots.Add(SubGroupKey(ss.Id, day, interval));
				}
			}
		}

		void ReserveSubGroupsForGroup(StudentGroup group, HourDay day, string interval)
		{
			if (group?.StudentSubGroups == null) return;
			foreach (var ss in group.StudentSubGroups)
			{
				occupiedSubGroupSlots.Add(SubGroupKey(ss.Id, day, interval));
			}
		}

		// ✅ Process each promotion - generate hours for BOTH semesters of current year
		foreach (var promotion in promotions)
		{
			if (promotion?.Years == null || promotion.StudentGroups == null) continue;

			// ✅ Determine current year for THIS promotion
			var currentYearNum = Math.Clamp(HelperFunctions.GetCurrentStudentYear(promotion.StartYear), 1, 3);

			// ✅ Get the CURRENT year
			var currentYear = promotion.Years.FirstOrDefault(y => y.YearNumber == currentYearNum);
			if (currentYear?.PromotionSemesters == null) continue;

			// ✅ Generate hours for BOTH semesters (1 and 2) of the current year
			foreach (var semester in currentYear.PromotionSemesters.OrderBy(s => s.SemesterNumber))
			{
				// ✅ Pick 5 subjects for this semester (random from all available)
				var semesterSubjects = subjects.OrderBy(_ => _random.Next()).Take(5).ToList();
				if (semesterSubjects.Count == 0) continue;

				var facultyId = promotion.Specialisation?.Faculty?.Id ?? 0;
				var candidateTeachers = teachers.Where(t => t.FacultyId == facultyId).ToList();
				if (candidateTeachers.Count == 0)
					candidateTeachers = teachers.ToList();

				// ✅ Generate hours for each of the 5 subjects
				foreach (var subject in semesterSubjects)
				{
					// 1) Lecture for entire promotion (all groups/subgroups attend)
					{
						var teacher = candidateTeachers[_random.Next(candidateTeachers.Count)];
						if (TryFindSlotAndClassroom((d, it) =>
							!occupiedTeacherSlots.Contains(TeacherKey(teacher.Id, d, it))
							&& !AnySubGroupOccupiedForPromotion(promotion, d, it)
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
								Promotion = promotion,
								PromotionId = promotion.Id,
								Semester = semester,
								SemesterId = semester.Id,
								Category = HourCategory.Lecture
							};

							hoursToAdd.Add(h);
							occupiedTeacherSlots.Add(TeacherKey(teacher.Id, day, interval));
							occupiedClassroomSlots.Add(ClassroomKey(classroom.Id, day, interval));
							ReserveSubGroupsForPromotion(promotion, day, interval);
						}
					}

					// 2) Seminars for each group (75% weekly, else 50/50 First/Second week)
					foreach (var group in promotion.StudentGroups)
					{
						var p = _random.NextDouble();
						HourFrequency freq = p < 0.75 ? HourFrequency.Weekly
							: _random.NextDouble() < 0.5 ? HourFrequency.FirstWeek : HourFrequency.SecondWeek;

						var teacher = candidateTeachers[_random.Next(candidateTeachers.Count)];

						if (TryFindSlotAndClassroom((d, it) =>
							!AnySubGroupOccupiedForGroup(group, d, it)
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
								Semester = semester,
								SemesterId = semester.Id,
								Category = HourCategory.Seminar
							};

							hoursToAdd.Add(h);
							ReserveSubGroupsForGroup(group, day, interval);
							occupiedTeacherSlots.Add(TeacherKey(teacher.Id, day, interval));
							occupiedClassroomSlots.Add(ClassroomKey(classroom.Id, day, interval));
						}
					}

					// 3) Labs for each subgroup (50% weekly, else 25/25 first/second week)
					foreach (var group in promotion.StudentGroups)
					{
						if (group?.StudentSubGroups == null) continue;
						foreach (var sub in group.StudentSubGroups)
						{
							var r = _random.NextDouble();
							HourFrequency freq = r < 0.5 ? HourFrequency.Weekly
								: _random.NextDouble() < 0.5 ? HourFrequency.FirstWeek : HourFrequency.SecondWeek;

							var teacher = candidateTeachers[_random.Next(candidateTeachers.Count)];

							if (TryFindSlotAndClassroom((d, it) =>
								!occupiedSubGroupSlots.Contains(SubGroupKey(sub.Id, d, it))
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
									Semester = semester,
									SemesterId = semester.Id,
									Category = HourCategory.Laboratory
								};

								hoursToAdd.Add(h);
								occupiedSubGroupSlots.Add(SubGroupKey(sub.Id, day, interval));
								occupiedTeacherSlots.Add(TeacherKey(teacher.Id, day, interval));
								occupiedClassroomSlots.Add(ClassroomKey(classroom.Id, day, interval));
							}
						}
					}
				}
			}
		}

		if (hoursToAdd.Count != 0)
		{
			await _context.Hours.AddRangeAsync(hoursToAdd);
			await _context.SaveChangesAsync();
		}
	}
}