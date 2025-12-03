using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Repository.Context;
using EFUser = TrackForUBB.Repository.EFEntities.User;
using EntraUser = Microsoft.Graph.Models.User;

namespace TrackForUBB.Repository.DataSeeder;
public class MicrosoftEntraUserDataSeeder(AcademicAppContext context, GraphServiceClient graph)
{
    private readonly AcademicAppContext _context = context;
    private readonly GraphServiceClient _graph = graph;
    private readonly ILog _logger = LogManager.GetLogger(typeof(MicrosoftEntraUserDataSeeder));

    private async Task<List<EFUser>> GetRandomStudents()
    {
        return await _context.Users
            .Where(u => u.Enrollments.Any() && !_context.Teachers.Any(t => t.UserId == u.Id) && u.Role == UserRole.Student)
            .OrderBy(x => Guid.NewGuid())
            .Take(5)
            .ToListAsync();
    }

    private async Task<List<EFUser>> GetRandomTeachers()
    {
        return await _context.Users
            .Where(u => !u.Enrollments.Any() && _context.Teachers.Any(t => t.UserId == u.Id) && u.Role == UserRole.Teacher)
            .OrderBy(x => Guid.NewGuid())
            .Take(5)
            .ToListAsync();
    }

    public async Task SeedAsync()
    {
        var count = await _graph.Users.Count.GetAsync((requestConfiguration) =>
        {
            requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
        });

        _logger.InfoFormat("Number of users in entra: {0}", count);

        if (count > 3 || _context.Users.Count() < 11)
        {
            return;
        }

        var students = await GetRandomStudents();
        var teachers = await GetRandomTeachers();

        if (students.Any(x => x.FirstName == "" || x.LastName == ""))
        {
            _logger.Warn("Some students have missing first name or last name. Skipping seeding.");
            return;
        }

        if (teachers.Any(x => x.FirstName == "" || x.LastName == ""))
        {
            _logger.Warn("Some teachers have missing first name or last name. Skipping seeding.");
            return;
        }

        foreach (var student in students)
        {
            var requestBody = new EntraUser
            {
                AccountEnabled = true,
                DisplayName = $"{student.LastName} {student.FirstName}",
                MailNickname = $"{student.LastName}.{student.FirstName}",
                UserPrincipalName = $"{student.FirstName}.{student.LastName}@trackforubb.onmicrosoft.com",
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = true,
                    Password = "Parola1234!",
                },
            };

            var result = await _graph.Users.PostAsync(requestBody);

            if (result != null && result.Id != null)
            {
                student.Owner = Guid.Parse(result.Id);
            }
        }

        foreach (var teacher in teachers)
        {
            var requestBody = new EntraUser
            {
                AccountEnabled = true,
                DisplayName = $"{teacher.LastName} {teacher.FirstName}",
                MailNickname = $"{teacher.LastName}.{teacher.FirstName}",
                UserPrincipalName = $"{teacher.FirstName}.{teacher.LastName}@trackforubb.onmicrosoft.com",
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = true,
                    Password = "Parola1234!",
                },
            };

            var result = await _graph.Users.PostAsync(requestBody);

            if (result != null && result.Id != null)
            {
                teacher.Owner = Guid.Parse(result.Id);
            }
        }

        await _context.SaveChangesAsync();
    }
}
