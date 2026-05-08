using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Repository
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlMessage);



        Task SendEmailWithTemplateAsync(string email, string subject, string templateFileName, object model);
    }
}
