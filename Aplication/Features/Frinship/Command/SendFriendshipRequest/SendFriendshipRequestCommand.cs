using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Aplication.ResponPattern;
using System.Security.Claims;
namespace Aplication.Features.Frinship.Command.SendFriendshipRequest
{
    public class SendFriendshipRequestCommand : IRequest<Response<bool>>
    {
        public ClaimsPrincipal Principal { get; set; }
       
        public string ProfileIdReseptor { get; set; }
        


    }
}
