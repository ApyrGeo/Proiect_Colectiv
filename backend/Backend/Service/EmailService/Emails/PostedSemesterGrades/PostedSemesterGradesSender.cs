using System.Net;
using TrackForUBB.Service.EmailService.Abstract;
using TrackForUBB.Service.EmailService.Configuration;
using TrackForUBB.Service.EmailService.Interfaces;
using TrackForUBB.Service.EmailService.Models;

namespace TrackForUBB.Service.EmailService.Emails.PostedSemesterGrades;

public class PostedSemesterGradesSender(EmailSettings config) : BaseEmailSender<PostedSemesterGradesModel>(config), IEmailSender<PostedSemesterGradesModel>
{
    private const string TemplatePath = "Grades\\SemesterGradesEmailPage.html";

    public override async Task SendEmailAsync(string to, PostedSemesterGradesModel model)
    {
        string html = await LoadTemplateAsync(TemplatePath);

        html = html.Replace("{{UserFirstName}}", WebUtility.HtmlEncode(model.UserFirstName))
            .Replace("{{UserLastName}}", WebUtility.HtmlEncode(model.UserLastName))
            .Replace("{{YearOfStudy}}", WebUtility.HtmlEncode(model.YearOfStudy.ToString()))
            .Replace("{{SemesterNumber}}", WebUtility.HtmlEncode(model.SemesterNumber.ToString()))
            .Replace("{{BaseUrl}}", WebUtility.HtmlEncode(_baseUrl));

        var message = new EmailMessage
        {
            To = to,
            Subject = $"Grades Posted for {model.UserFirstName} - Year {model.YearOfStudy}, Semester {model.SemesterNumber}",
            HtmlBody = html
        };

        await SendAsync(message);
    }
    
}