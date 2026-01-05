using System.Net;
using TrackForUBB.Service.EmailService.Abstract;
using TrackForUBB.Service.EmailService.Configuration;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;

namespace TrackForUBB.Service.EmailService.Emails.CreatedAccount;

public class CreatedAccountEmailSender(EmailSettings config) : BaseEmailSender<CreatedUserModel>(config), IEmailSender<CreatedUserModel>
{
    private static readonly string TemplatePath = Path.Combine("CreatedAccount", "CreatedAccountEmailPage.html");

    public override async Task SendEmailAsync(string to, CreatedUserModel model)
    {
        string html = await LoadTemplateAsync(TemplatePath);

        html = html.Replace("{{UserName}}", WebUtility.HtmlEncode(model.FirstName + " " + model.LastName))
            .Replace("{{Password}}", WebUtility.HtmlEncode(model.Password))
            .Replace("{{BaseUrl}}", WebUtility.HtmlEncode(_baseUrl));

        var message = new EmailMessage
        {
            To = to,
            Subject = $"Welcome to TrackForUBB, {model.FirstName}!",
            HtmlBody = html
        };

        await SendAsync(message);
    }
}
