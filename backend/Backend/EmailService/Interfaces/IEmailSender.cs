using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Interfaces;

public interface IEmailSender<T>
{
    Task SendEmailAsync(string to, T model);
}
