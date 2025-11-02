using Backend.Context;
using Backend.Domain;
using Backend.Domain.Enums;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataSeeder;

public class UserDataSeeder
{
    private readonly AcademicAppContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserDataSeeder(AcademicAppContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        //populate only empty DB
        if (await _context.Users.AnyAsync())
            return;

        var subGroups = await _context.SubGroups.ToListAsync();
        var subGroupsCounts = subGroups.ToDictionary(sg => sg.Id, sg => 0);

        var faker = new Faker<User>("ro").UseSeed(6767);

        int totalUsers = 10000;
        int minStudentsPerSubGroup = 14;

        var userFaker = faker
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.PhoneNumber, (f, u) => $"+40 {f.Random.Number(700, 799)} {f.UniqueIndex:D3} {f.Random.Number(100, 999)}")
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName, "gmoil.com", f.UniqueIndex.ToString())) 
            .RuleFor(u => u.Password, (f,u) => _passwordHasher.HashPassword(u, "Password123!"))
            .RuleFor(u => u.Role, f => f.Random.Double() < 0.10 ? UserRole.Teacher : UserRole.Student)
            .RuleFor(u => u.Enrollments, _ => []);

        var users = userFaker.Generate(totalUsers - 1);

        users.Add(new User
        {
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com",
            PhoneNumber = "+40 700 000 000",
            Password = "AdminPassword123!",
            Role = UserRole.Admin,
            Enrollments = []
        });

        var students = users.Where(u => u.Role == UserRole.Student).ToList();
        var random = new Random();


        foreach (var student in students)
        {
            //1st enrollment - "compulsory"
            var sg1 = subGroups[random.Next(subGroups.Count)];
            student.Enrollments.Add(new Enrollment
            {
                SubGroup = sg1,
                User = student,
            });
            subGroupsCounts[sg1.Id]++;

            //2nd enrollment - optional - 20% chances
            if(random.NextDouble() < 0.2)
            {
                var otherSubGroups = subGroups.Where(s => s.Id != sg1.Id && s.StudentGroup.GroupYear.Specialisation.FacultyId != sg1.StudentGroup.GroupYear.Specialisation.FacultyId).ToList();
                var sg2 = otherSubGroups[random.Next(otherSubGroups.Count)];
                student.Enrollments.Add(new Enrollment
                {
                    SubGroup = sg2,
                    User = student,
                });
                subGroupsCounts[sg2.Id]++;
            }
        }

        //ensure all subgroups are filled
        foreach(var sg  in subGroups)
        {
            while (subGroupsCounts[sg.Id] < minStudentsPerSubGroup)
            {
                var eligible = students.Where(s => !s.Enrollments.Any(e => e.SubGroupId == sg.Id || e.SubGroup.StudentGroup.GroupYear.Specialisation.FacultyId == sg.StudentGroup.GroupYear.Specialisation.FacultyId)).ToList();
                if (!eligible.Any()) break;

                var rnd = new Random();
                var student = eligible[rnd.Next(eligible.Count)];
                student.Enrollments.Add(new Enrollment
                {
                    SubGroup = sg,
                    User = student
                });
                subGroupsCounts[sg.Id]++;
            }
        }

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }
}
