using EmailService.Abstract;
using EmailService.Configuration;
using EmailService.Interfaces;
using EmailService.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Emails.CreatedEnrollment;

public class CreatedEnrollmentEmailSender(EmailSettings config) : BaseEmailSender<CreatedEnrollmentModel>(config), IEmailSender<CreatedEnrollmentModel>
{
    private const string TemplatePath = "CreatedEnrollment\\CreatedEnrollmentEmailPage.html";

    public override async Task SendEmailAsync(string to, CreatedEnrollmentModel model)
    {
        string html = await LoadTemplateAsync(TemplatePath);

        html = html.Replace("{{UserFirstName}}", WebUtility.HtmlEncode(model.UserFirstName))
            .Replace("{{UserLastName}}", WebUtility.HtmlEncode(model.UserLastName))
            .Replace("{{GroupName}}", WebUtility.HtmlEncode(model.GroupName))
            .Replace("{{BaseUrl}}", WebUtility.HtmlEncode(_baseUrl) ); //TODO: change url to match FE corresponding page

        var message = new EmailMessage
        {
            To = to,
            Subject = $"Created Enrollment for {model.UserFirstName}",
            HtmlBody = html
        };

        await SendAsync(message);
    }
}
