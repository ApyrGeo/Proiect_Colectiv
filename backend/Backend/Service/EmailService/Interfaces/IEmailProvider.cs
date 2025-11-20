using TrackForUBB.Service.EmailService.Models;

namespace TrackForUBB.Service.EmailService.Interfaces;

public interface IEmailProvider
{
    Task SendCreateAccountEmailAsync(string to, CreatedUserModel model);
    Task SendCreateEnrollmentEmailAsync(string to, CreatedEnrollmentModel model);
}