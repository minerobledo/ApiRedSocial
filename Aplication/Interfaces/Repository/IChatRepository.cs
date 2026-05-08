using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Chats;

namespace Aplication.Interfaces.Repository
{
    public interface IChatRepository
    {
        Task<ChatDocument?> CreateChatAsync(string profile1, string profile2, string profile1Name, string profile2Name);
        Task<ChatDocument?> getChatbByTwoProfil(string Profile1, string Profile2);
        Task<List<ChatDocument>?> GetChatsByUserIdAsync(string profileId);
       
    }
}
