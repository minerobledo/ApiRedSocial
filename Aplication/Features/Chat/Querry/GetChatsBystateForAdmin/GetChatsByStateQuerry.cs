using Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Querry.GetChatsBystateForAdmin
{
    public class GetChatsByStateQuerry: IRequest<Response<List<AdminChatDocument>>>
    {
        public DateTime? StartAfter { get; set; } = null;
        public string? State { get; set; } = null;
        public string? Subject { get; set; } = null;
        public string? ProfileId { get; set; } = null;
        public string? ProfileName { get; set; } = null;

    }
}
