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

namespace EmailService.Emails.CreatedAccount;

public class CreatedAccountEmailSender(EmailSettings config) : BaseEmailSender<CreatedUserModel>(config), IEmailSender<CreatedUserModel>
{
    private const string TemplatePath = "CreatedAccount\\CreatedAccountEmailPage.html";

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
