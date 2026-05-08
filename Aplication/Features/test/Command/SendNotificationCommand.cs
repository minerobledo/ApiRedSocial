using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Aplication.DTO.Test;

namespace Aplication.Features.test.Command
{
    public class SendNotificationCommand : IRequest<string>
    {
        public NotificationRequest notificationRequest {  get; set; }
    }
}
