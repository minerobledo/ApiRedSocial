using Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Querry.GetChatsByProfile
{
    public class GetChatsByProfileQuerry : IRequest<Response<List<ChatDocument>>>
    {
        public ClaimsPrincipal? principal { get; set; } = null;

    }
}
