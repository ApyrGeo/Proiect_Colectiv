using EmailService.Models;

namespace EmailService.Interfaces;

public interface IEmailProvider
{
    Task SendCreateAccountEmailAsync(string to, CreatedUserModel model);
    Task SendCreateEnrollmentEmailAsync(string to, CreatedEnrollmentModel model);
}