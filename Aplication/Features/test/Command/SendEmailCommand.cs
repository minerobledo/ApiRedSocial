using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Aplication.ResponPattern;

namespace Aplication.Features.test.Command
{
    public class SendEmailCommand : IRequest<Response<bool>>
    {
        public string Email { get; set; }
    }
}
