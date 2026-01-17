using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using TrackForUBB.Domain.Enums;
using TrackForUBB.Domain.Utils;
using TrackForUBB.Repository.Context;
using EFUser = TrackForUBB.Repository.EFEntities.User;
using EntraUser = Microsoft.Graph.Models.User;

namespace TrackForUBB.Repository.DataSeeder;
public class MicrosoftEntraUserDataSeeder(AcademicAppContext context, GraphServiceClient graph, IConfiguration config)
{
    private readonly ILog _logger = LogManager.GetLogger(typeof(MicrosoftEntraUserDataSeeder));

    private readonly AcademicAppContext _context = context;
    private readonly GraphServiceClient _graph = graph;
    private readonly IConfiguration _config = config;

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

        var resourceId = _config["AzureAd:ResourceId"];
        var studentRoleId = _config["AzureAd:AppRoles:Student"];
        var teacherRoleId = _config["AzureAd:AppRoles:Teacher"];
        var defaultPassword = _config["EntraUserDefaultPassword"];

        if (resourceId == null || studentRoleId == null || teacherRoleId == null || defaultPassword == null)
        {
            _logger.Error("Missing configuration for Entra user seeding.");
            return;
        }

        var students = await GetRandomStudents();
        var teachers = await GetRandomTeachers();

        foreach (var student in students)
        {
            var mailNick = HelperFunctions.ReplaceRomanianDiacritics($"{student.FirstName}.{student.LastName}".ToLowerInvariant());
            var userPrincipal = $"{mailNick}@trackforubb.onmicrosoft.com";

            var entraUserRequestBody = new EntraUser
            {
                AccountEnabled = true,
                DisplayName = $"{student.FirstName} {student.LastName}",
                MailNickname = mailNick,
                UserPrincipalName = userPrincipal,
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = true,
                    Password = defaultPassword,
                },
            };

            var result = await _graph.Users.PostAsync(entraUserRequestBody);

            if (result != null && result.Id != null)
            {
                student.Owner = Guid.Parse(result.Id);

                var appRoleAssigmentRequestBody = new AppRoleAssignment
                {
                    PrincipalId = student.Owner,
                    ResourceId = Guid.Parse(resourceId),
                    AppRoleId = Guid.Parse(studentRoleId),
                };

                await _graph.Users[$"{student.Owner}"].AppRoleAssignments.PostAsync(appRoleAssigmentRequestBody);
            }
        }

        foreach (var teacher in teachers)
        {
            var mailNick = HelperFunctions.ReplaceRomanianDiacritics($"{teacher.FirstName}.{teacher.LastName}".ToLowerInvariant());
            var userPrincipal = $"{mailNick}@trackforubb.onmicrosoft.com";

            var requestBody = new EntraUser
            {
                AccountEnabled = true,
                DisplayName = $"{teacher.FirstName} {teacher.LastName}",
                MailNickname = mailNick,
                UserPrincipalName = userPrincipal,
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = true,
                    Password = defaultPassword,
                },
            };

            var result = await _graph.Users.PostAsync(requestBody);

            if (result != null && result.Id != null)
            {
                teacher.Owner = Guid.Parse(result.Id);

                var appRoleAssigmentRequestBody = new AppRoleAssignment
                {
                    PrincipalId = teacher.Owner,
                    ResourceId = Guid.Parse(resourceId),
                    AppRoleId = Guid.Parse(teacherRoleId),
                };

                await _graph.Users[$"{teacher.Owner}"].AppRoleAssignments.PostAsync(appRoleAssigmentRequestBody);
            }
        }

        await _context.SaveChangesAsync();
    }
}
