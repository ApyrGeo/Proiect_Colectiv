using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using EmailService.Models;

namespace EmailService.Abstract;

public abstract class BaseEmailSender<T>
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromAddress;
    protected readonly string _baseUrl;

    protected readonly string BasePath;

    //configure shared info between different email types
    protected BaseEmailSender(IConfiguration config)
    {
        _fromAddress = config["Email:From"]!;
        _smtpClient = new(config["Email:Smtp:Host"]!)
        {
            Port = int.Parse(config["Email:Smtp:Port"]!),
            Credentials = new NetworkCredential(
                config["Email:Smtp:Username"],
                config["Email:Smtp:Password"]
                ),
            EnableSsl = true
        };
        _baseUrl = config["Email:BaseUrl"]!;

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
