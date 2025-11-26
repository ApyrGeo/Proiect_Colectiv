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
		//populate only empty DB
		if (await _context.Users.AnyAsync())
			return;

		var subGroups = await _context.SubGroups
			.Include(s => s.StudentGroup)
				.ThenInclude(sg => sg.Promotion)
					.ThenInclude(p => p.Specialisation)
			.Include(s => s.StudentGroup)
				.ThenInclude(sg => sg.Promotion)
					.ThenInclude(p => p.Years)
						.ThenInclude(py => py.PromotionSemesters)
							.ThenInclude(ps => ps.Contracts)
								.ThenInclude(ps => ps.Subjects)
			.ToListAsync();
		var subGroupsCounts = subGroups.ToDictionary(sg => sg.Id, sg => 0);

		var faker = new Faker<User>("ro").UseSeed(6767);

		int totalUsers = 1000;
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

		// helper to create contract for an enrollment
		async Task CreateContractForEnrollmentAsync(Enrollment enrollment, Promotion promotion)
		{
			// determine current semester for the promotion
			var currentYearNum = Math.Clamp(HelperFunctions.GetCurrentStudentYear(promotion.StartYear), 1, 3);
			var currentSemesterNum = DateTime.Now.Month < 7 ? 1 : 2;
			var currentYear = promotion.Years.FirstOrDefault(y => y.YearNumber == currentYearNum);
			var currentSemester = currentYear?.PromotionSemesters.FirstOrDefault(s => s.SemesterNumber == currentSemesterNum);

			if (currentSemester == null)
				return;

			// pick available subjects for this semester (fallback to promotion-level subjects)
			var available = currentSemester.Contracts.SelectMany(c => c.Subjects).ToList() ?? new List<Subject>();
			if (!available.Any())
			{
				available = await _context.Subjects
					.Include(s => s.Contracts)
						.ThenInclude(c => c.Semester)
							.ThenInclude(ps => ps.PromotionYear)
								.ThenInclude(py => py.Promotion)
					.Where(s => s.Contracts.Any(c => c.Semester != null && c.Semester.PromotionYear.Promotion.Id == promotion.Id))
					.ToListAsync();
			}

			if (!available.Any())
				return;

			int take = random.Next(4, Math.Min(available.Count, 8) + 1);
			var chosen = available.OrderBy(_ => random.Next()).Take(take).ToList();

			var contract = new Contract
			{
				Semester = currentSemester,
				SemesterId = currentSemester.Id,
				Enrollment = enrollment,
				Subjects = chosen
			};

			// ensure enrollment.Contracts collection exists and add
			if (enrollment.Contracts == null)
				enrollment.Contracts = new List<Contract>();
			enrollment.Contracts.Add(contract);
		}

		foreach (var student in students)
		{
			//1st enrollment - "compulsory"
			var sg1 = subGroups[random.Next(subGroups.Count)];

			var promotion = sg1.StudentGroup.Promotion;
			// create enrollment
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

			// create contract for this enrollment
			await CreateContractForEnrollmentAsync(enrollment1, promotion);

			//2nd enrollment - optional - 20% chances
			if (random.NextDouble() < 0.2)
			{
				var otherSubGroups = subGroups
					.Where(s => s.Id != sg1.Id && s.StudentGroup.Promotion.Specialisation.FacultyId != sg1.StudentGroup.Promotion.Specialisation.FacultyId)
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

					// create contract for second enrollment
					await CreateContractForEnrollmentAsync(enrollment2, promotion2);
				}
			}
		}

		//ensure all subgroups are filled
		foreach (var sg in subGroups)
		{
			while (subGroupsCounts[sg.Id] < minStudentsPerSubGroup)
			{
				var eligible = students.Where(s => !s.Enrollments.Any(e => e.SubGroupId == sg.Id || e.SubGroup.StudentGroup.Promotion.Specialisation.FacultyId == sg.StudentGroup.Promotion.Specialisation.FacultyId)).ToList();
				if (!eligible.Any()) break;

				var rndIdx = new Random();
				var student = eligible[rndIdx.Next(eligible.Count)];

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

				await CreateContractForEnrollmentAsync(enrollment, promotion);
			}
		}

		await _context.Users.AddRangeAsync(users);
		await _context.SaveChangesAsync();
	}
}