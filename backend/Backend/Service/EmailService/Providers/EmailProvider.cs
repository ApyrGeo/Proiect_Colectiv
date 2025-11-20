using TrackForUBB.Service.EmailService.Configuration;
using TrackForUBB.Service.EmailService.Emails.CreatedAccount;
using TrackForUBB.Service.EmailService.Emails.CreatedEnrollment;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;

namespace TrackForUBB.Service.EmailService.Providers;

public class EmailProvider : IEmailProvider
{
    private readonly EmailSettings _config;

    public EmailProvider(EmailSettings config)
    {
        _config = config;
    }

    public async Task SendCreateAccountEmailAsync(string to, CreatedUserModel model)
    {
        var createdAccount = new CreatedAccountEmailSender(_config);
        await createdAccount.SendEmailAsync(to, model);
    }

    public async Task SendCreateEnrollmentEmailAsync(string to, CreatedEnrollmentModel model)
    {
        var createdEnrollment = new CreatedEnrollmentEmailSender(_config);
        await createdEnrollment.SendEmailAsync(to, model);
    }
}
