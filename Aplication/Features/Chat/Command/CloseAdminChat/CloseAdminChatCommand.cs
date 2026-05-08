using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Command.CloseAdminChat
{
    public class CloseAdminChatCommand : IRequest<Response<bool?>>
    {
        public string? ChatId { get; set; }
    }
}
