using Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Repository
{
    public interface IAdminChatRepository
    {
        Task<bool?> ExistAdminChat(string profileId);
        Task<AdminChatDocument?> CreateAdminChatAsync(string profileId, string ProfileName , string Subject);
        Task<List<AdminChatDocument>?> GetAllChatWithAdmin(string profile1, DateTime? stardAfter);
        Task<AdminChatDocument?> GetActiveChatWithAdmin(string profile1);
        Task<bool?> ActiveChatWithAdmin(string ChatId, string AdminID, string AdminFullName);
        Task<bool?> CloseChatWithAdmin(string ChatId);
        
        Task<List<AdminChatDocument>?> GetAllChatsFilteredAsync(DateTime? startAfter, string? state = null, string? subject = null, string? profileId = null, string? profileName = null);
    }
}
