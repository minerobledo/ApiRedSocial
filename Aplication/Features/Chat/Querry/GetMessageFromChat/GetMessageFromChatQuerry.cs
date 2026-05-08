using Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Aplication.Features.Chat.Querry.GetMessageFromChat
{
    public class GetMessageFromChatQuerry: IRequest <Response<List<ChatMessage>?>>
    {
        public DateTime? StartAfeter { get; set; } = null;
        public string? chatId {  get; set; }
        public string? Type { get; set; }
    }
}
