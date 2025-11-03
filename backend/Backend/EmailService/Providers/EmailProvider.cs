using EmailService.Configuration;
using EmailService.Emails.CreatedAccount;
using EmailService.Emails.CreatedEnrollment;
using EmailService.Models;

namespace EmailService.Providers;

public class EmailProvider(EmailSettings config)
{
    private readonly EmailSettings _config = config;

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
