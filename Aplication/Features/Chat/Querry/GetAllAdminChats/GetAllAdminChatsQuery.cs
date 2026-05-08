using Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Querry.GetAllAdminChats
{
    public class GetAllAdminChatsQuery: IRequest<Response<List<AdminChatDocument>>>
    {
        public ClaimsPrincipal? Principal { get; set; }
        public DateTime? StartAfter { get; set; } = null;
    }
}
