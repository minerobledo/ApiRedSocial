using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Google.Cloud.Firestore;


namespace Aplication.Interfaces.Repository
{
    public interface IFriendshipRepository
    {
        Task<Friendship?> GetFriendshipAsinc(string id);
        Task<Friendship?> GetFriendshipAsinc(string profileId1, string profileId2);
        string? AddFrienshipTransaction(Transaction transaction, string profilIdSender, string profilIDResiver,string nameSender,string nameReseptor);
        Task<List<Friendship>?> GetAllFriendshipByProfilIdAsinc(string id);
        Task<List<string>?> GetAllFriendsIDpByProfilIdAsinc(string profileId);
        Task<bool?> ChangeStatusFrienship(string ID, bool status);
        
        Task<bool?> ExistFrienship(string Id1, string Id2);
    }
}
