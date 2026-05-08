using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;


using Aplication.ResponPattern;
using System.Security.Claims;
using Domain.Entities.Notification;

namespace Aplication.Features.Notificatoins.Query
{
    public class GetNotificationByProfilIdQuery : IRequest<Response<List<NotificationEntity>>>
    {
        public ClaimsPrincipal? Principal { get; set; }


    }
}
