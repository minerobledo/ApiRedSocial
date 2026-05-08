using Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Command.CreateChat
{
    public class CreateChatCommand : IRequest<Response<ChatDocument?>>
    {
        public ClaimsPrincipal? Principal { get; set; } = null;
        public string? SelfProfileName { get; set; }
        public string? Profile2Id { get; set; }
        public string? FriendProfileName { get; set; }

       
    }
}
