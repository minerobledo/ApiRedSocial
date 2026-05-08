using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Aplication.ResponPattern;
using Aplication.Interfaces.Repository;

namespace Aplication.Features.test.Command
{
    public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, Response<bool>>
    {
        private readonly IEmailService _emailService;
        public SendEmailCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }


        public async Task<Response<bool>> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            await _emailService.SendEmailWithTemplateAsync(
                request.Email,
                "Bienvenido a nuestra plataforma",
                "confirm_email_template.html",
                 new { senderNickname = "Juan", button_link = "https://www.youtube.com" }

            );
            return new Response<bool>
            {
                succeeded = true,
                data = true
            };
        }
    }
}
