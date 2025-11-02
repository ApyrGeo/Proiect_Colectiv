using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Configuration;

public class EmailSettings
{
    public string From { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public SmtpSettings Smtp { get; set; } = new();
    public string BaseUrl { get; set; } = string.Empty;
}

public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
}
