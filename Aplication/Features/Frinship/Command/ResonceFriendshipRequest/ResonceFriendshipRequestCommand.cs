using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Aplication.ResponPattern;
using Domain.Entities;
using MediatR;

namespace Aplication.Features.Frinship.Command.ResonceFriendshipRequest
{
    public class ResonceFriendshipRequestCommand : IRequest<Response<bool>>
    {
        public ClaimsPrincipal principal { get; set; }
        public bool? Responce {  get; set; }
        public string? FriendshipId {  get; set; }
    }
}
