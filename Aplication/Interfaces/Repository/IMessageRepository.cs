using Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Aplication.Interfaces.Repository
{
    public interface IMessageRepository
    {

        Task<List<ChatMessage>?> GetAdminMessagesByChatIdAsync(string chatId, DateTime? startAfter = null);
        Task<bool?> SaveAdminMessageAsync(ChatMessage message, string chatId);
        Task<List<ChatMessage>?> GetMessagesByChatIdAsync(string chatId, DateTime? startAfter = null);
        
        Task<bool?> SaveMessageAsync(ChatMessage message, string chatId);
    }
}

