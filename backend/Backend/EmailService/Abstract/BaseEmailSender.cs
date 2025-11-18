using System.Net;
using System.Net.Mail;
using EmailService.Models;
using EmailService.Configuration;

namespace EmailService.Abstract;

public abstract class BaseEmailSender<T>
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromAddress;
    protected readonly string _baseUrl;

    protected readonly string BasePath;

    //configure shared info between different email types
    protected BaseEmailSender(EmailSettings config)
    {
        _fromAddress = config.From;
        _smtpClient = new(config.Smtp.Host)
        {
            Port = config.Smtp.Port,
            Credentials = new NetworkCredential(
                config.Smtp.Username,
                config.Smtp.Password
                ),
            EnableSsl = true
        };
        _baseUrl = config.BaseUrl;

        BasePath = Path.Combine(AppContext.BaseDirectory, "Emails");
    }

    protected async Task<string> LoadTemplateAsync(string relativePath)
    {
        var fullPath = Path.Combine(BasePath, relativePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Email template not found: {fullPath}");

        return await File.ReadAllTextAsync(fullPath);
    }

    protected async Task SendAsync(EmailMessage message)
    {
        using var mail = new MailMessage
        {
            From = new MailAddress(_fromAddress),
            Subject = message.Subject,
            Body = message.HtmlBody,
            IsBodyHtml = true
        };

        mail.To.Add(message.To);

        await _smtpClient.SendMailAsync(mail);
    }

    public abstract Task SendEmailAsync(string to, T model);
}
