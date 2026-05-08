using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Command.ActiveAdminChat
{
    public class ActiveAdminChatCommand:IRequest <Response<bool?>>
    {
        public string? ChatId { get; set; }
        public string? AdminID {  get; set; }
        public string? AdminFullName { get; set; }
    }
}
