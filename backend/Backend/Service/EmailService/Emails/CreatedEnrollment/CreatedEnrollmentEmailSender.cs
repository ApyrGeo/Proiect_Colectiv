using System.Net;
using TrackForUBB.Service.EmailService.Abstract;
using TrackForUBB.Service.EmailService.Configuration;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;

namespace TrackForUBB.Service.EmailService.Emails.CreatedEnrollment;

public class CreatedEnrollmentEmailSender(EmailSettings config) : BaseEmailSender<CreatedEnrollmentModel>(config), IEmailSender<CreatedEnrollmentModel>
{
    private static readonly string TemplatePath = Path.Combine("CreatedEnrollment", "CreatedEnrollmentEmailPage.html");

    public override async Task SendEmailAsync(string to, CreatedEnrollmentModel model)
    {
        string html = await LoadTemplateAsync(TemplatePath);

        html = html.Replace("{{UserFirstName}}", WebUtility.HtmlEncode(model.UserFirstName))
            .Replace("{{UserLastName}}", WebUtility.HtmlEncode(model.UserLastName))
            .Replace("{{GroupName}}", WebUtility.HtmlEncode(model.GroupName))
            .Replace("{{BaseUrl}}", WebUtility.HtmlEncode(_baseUrl)); //TODO: change url to match FE corresponding page

        var message = new EmailMessage
        {
            To = to,
            Subject = $"Created Enrollment for {model.UserFirstName}",
            HtmlBody = html
        };

        await SendAsync(message);
    }
}
