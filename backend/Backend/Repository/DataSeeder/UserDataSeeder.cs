using TrackForUBB.Repository.Context;
using Bogus;
using Microsoft.EntityFrameworkCore;
using TrackForUBB.Domain.Security;
using TrackForUBB.Repository.EFEntities;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Utils;

namespace TrackForUBB.Repository.DataSeeder;

public class UserDataSeeder
{
	private readonly AcademicAppContext _context;
	private readonly IAdapterPasswordHasher<User> _passwordHasher;

	public UserDataSeeder(AcademicAppContext context, IAdapterPasswordHasher<User> passwordHasher)
	{
		_context = context;
		_passwordHasher = passwordHasher;
	}

	public async Task SeedAsync()
	{
		if (await _context.Users.AnyAsync())
			return;

		// Load all subjects ONCE at the start
		var allSubjects = await _context.Subjects.ToListAsync();
		if (!allSubjects.Any())
		{
			// Log warning or throw - cannot create contracts without subjects
			Console.WriteLine("Warning: No subjects found. Contracts will be empty.");
		}

		var subGroups = await _context.SubGroups
			.Include(s => s.StudentGroup)
				.ThenInclude(sg => sg.Promotion)
					.ThenInclude(p => p.Specialisation)
			.Include(s => s.StudentGroup)
				.ThenInclude(sg => sg.Promotion)
					.ThenInclude(p => p.Years)
						.ThenInclude(py => py.PromotionSemesters)
			.ToListAsync();
		var subGroupsCounts = subGroups.ToDictionary(sg => sg.Id, sg => 0);

		var faker = new Faker<User>("ro").UseSeed(6767);

		int totalUsers = 10000;
		int minStudentsPerSubGroup = 14;

		var userFaker = faker
			.RuleFor(u => u.FirstName, f => f.Name.FirstName())
			.RuleFor(u => u.LastName, f => f.Name.LastName())
			.RuleFor(u => u.PhoneNumber, (f, u) => $"+40 {f.Random.Number(700, 799)} {f.UniqueIndex:D2} {f.Random.Number(100, 999)}")
			.RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName, "gmoil.com", f.UniqueIndex.ToString()))
			.RuleFor(u => u.Password, (f, u) => _passwordHasher.HashPassword(u, "Password123!"))
			.RuleFor(u => u.Role, f => f.Random.Double() < 0.10 ? UserRole.Teacher : UserRole.Student)
			.RuleFor(u => u.Enrollments, _ => new List<Enrollment>());

		var users = userFaker.Generate(totalUsers - 1);

		var admin = new User
		{
			FirstName = "Admin",
			LastName = "User",
			Email = "admin@example.com",
			PhoneNumber = "+40 700 000 000",
			Password = "AdminPassword123!",
			Role = UserRole.Admin,
			Enrollments = new List<Enrollment>()
		};

		admin.Password = _passwordHasher.HashPassword(admin, admin.Password);
		users.Add(admin);

		var students = users.Where(u => u.Role == UserRole.Student).ToList();
		var random = new Random();

		// Helper to create contract for an enrollment - NO async DB calls inside
		void CreateContractForEnrollment(Enrollment enrollment, Promotion promotion)
		{
			if (!allSubjects.Any())
				return;

			var currentYearNum = Math.Clamp(HelperFunctions.GetCurrentStudentYear(promotion.StartYear), 1, 3);
			var currentSemesterNum = DateTime.Now.Month < 7 ? 1 : 2;

			var currentYear = promotion.Years.FirstOrDefault(y => y.YearNumber == currentYearNum);
			var currentSemester = currentYear?.PromotionSemesters.FirstOrDefault(s => s.SemesterNumber == currentSemesterNum);

			if (currentSemester == null)
				return;

			int take = random.Next(4, Math.Min(allSubjects.Count, 8) + 1);
			var chosen = allSubjects.OrderBy(_ => random.Next()).Take(take).Distinct().ToList();

			var contract = new Contract
			{
				Semester = currentSemester,
				SemesterId = currentSemester.Id,
				Enrollment = enrollment,
				EnrollmentId = enrollment.Id,
				Subjects = chosen
			};

			enrollment.Contracts ??= new List<Contract>();
			enrollment.Contracts.Add(contract);
		}

		foreach (var student in students)
		{
			// 1st enrollment - "compulsory"
			var sg1 = subGroups[random.Next(subGroups.Count)];
			var promotion = sg1.StudentGroup.Promotion;

			var enrollment1 = new Enrollment
			{
				SubGroup = sg1,
				SubGroupId = sg1.Id,
				User = student,
				Grades = new List<Grade>(),
				Contracts = new List<Contract>()
			};

			student.Enrollments.Add(enrollment1);
			subGroupsCounts[sg1.Id]++;
			CreateContractForEnrollment(enrollment1, promotion);

			// 2nd enrollment - optional - 20% chances
			if (random.NextDouble() < 0.2)
			{
				var otherSubGroups = subGroups
					.Where(s => s.Id != sg1.Id &&
						   s.StudentGroup.Promotion.Specialisation.FacultyId != sg1.StudentGroup.Promotion.Specialisation.FacultyId)
					.ToList();

				if (otherSubGroups.Any())
				{
					var sg2 = otherSubGroups[random.Next(otherSubGroups.Count)];
					var promotion2 = sg2.StudentGroup.Promotion;

					var enrollment2 = new Enrollment
					{
						SubGroup = sg2,
						SubGroupId = sg2.Id,
						User = student,
						Grades = new List<Grade>(),
						Contracts = new List<Contract>()
					};

					student.Enrollments.Add(enrollment2);
					subGroupsCounts[sg2.Id]++;
					CreateContractForEnrollment(enrollment2, promotion2);
				}
			}
		}

		// Ensure all subgroups are filled
		foreach (var sg in subGroups)
		{
			while (subGroupsCounts[sg.Id] < minStudentsPerSubGroup)
			{
				var targetFacultyId = sg.StudentGroup.Promotion.Specialisation.FacultyId;

				var eligible = students.Where(s => !s.Enrollments.Any(e =>
				{
					if (e.SubGroupId == sg.Id)
						return true;

					var enrolledSubGroup = subGroups.FirstOrDefault(sub => sub.Id == e.SubGroupId);
					if (enrolledSubGroup == null)
						return false;

					var enrolledFacultyId = enrolledSubGroup.StudentGroup.Promotion.Specialisation.FacultyId;
					return enrolledFacultyId == targetFacultyId;
				})).ToList();

				if (!eligible.Any()) break;

				var student = eligible[random.Next(eligible.Count)];
				var promotion = sg.StudentGroup.Promotion;

				var enrollment = new Enrollment
				{
					SubGroup = sg,
					SubGroupId = sg.Id,
					User = student,
					Grades = new List<Grade>(),
					Contracts = new List<Contract>()
				};

				student.Enrollments.Add(enrollment);
				subGroupsCounts[sg.Id]++;
				CreateContractForEnrollment(enrollment, promotion);
			}
		}

		await _context.Users.AddRangeAsync(users);
		await _context.SaveChangesAsync();
	}
}