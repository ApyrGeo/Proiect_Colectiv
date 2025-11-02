using EmailService.Abstract;
using EmailService.Interfaces;
using EmailService.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Emails.CreatedAccount;

public class CreatedAccountEmailSender(IConfiguration config) : BaseEmailSender<CreatedUserModel>(config), IEmailSender<CreatedUserModel>
{
    private const string TemplatePath = "CreatedAccount\\CreatedAccountEmailPage.html";

    public override async Task SendEmailAsync(string to, CreatedUserModel model)
    {
        string html = await LoadTemplateAsync(TemplatePath);

        html = html.Replace("{{UserName}}", model.FirstName + " " + model.LastName)
            .Replace("{{Password}}", model.Password)
            .Replace("{{BaseUrl}}", _baseUrl);

        var message = new EmailMessage
        {
            To = to,
            Subject = $"Welcome to TrackForUBB, {model.FirstName}!",
            HtmlBody = html
        };

        await SendAsync(message);
    }
}
