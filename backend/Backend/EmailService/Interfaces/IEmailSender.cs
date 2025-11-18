namespace EmailService.Interfaces;

public interface IEmailSender<T>
{
    Task SendEmailAsync(string to, T model);
}
