using EmailService.Emails.CreatedAccount;
using EmailService.Emails.CreatedEnrollment;
using EmailService.Enums;
using EmailService.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.Providers;

public class EmailProvider(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public async Task SendAsync<T>(EmailType type, string to, T model)
    {
        switch (type)
        {
            case EmailType.CreatedAccount:
                var createdAccount = new CreatedAccountEmailSender(_config);
                var wantedUModel = model as CreatedUserModel;
                await createdAccount.SendEmailAsync(to, wantedUModel!);
                break;

            case EmailType.CreatedEnrollment:
                var createdEnrollment = new CreatedEnrollmentEmailSender(_config);
                var wantedEModel = model as CreatedEnrollmentModel;
                await createdEnrollment.SendEmailAsync(to, wantedEModel!);
                break;
            
            //other cases

            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}
