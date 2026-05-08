using Aplication.Interfaces.Repository;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Chats;
using Geohash;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal class AdminChatRepository: GenericRepository<AdminChatDocument>, IAdminChatRepository
    {
        public AdminChatRepository(FirestoreDb firestoreDb, string collectionName = "AdminChat") : base(firestoreDb, collectionName)
        {
            //establese la coneccion con la bace dedatos para esta instancia
        }

        public async Task<bool?> ExistAdminChat(string profileId)
        {
            if (string.IsNullOrWhiteSpace(profileId)) return null;
            try
            {
                var docRef =  _firestoreDb.Collection(_collectionName).WhereEqualTo("ProfileId", profileId);
                var snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Count != 0) return true;
                return false;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<AdminChatDocument?> CreateAdminChatAsync(string profileId, string ProfileName, string Subject)
        {
            if (string.IsNullOrEmpty(profileId) || string.IsNullOrEmpty(profileId) ) return null;
            try
            {
                var chatDoc = _firestoreDb.Collection(_collectionName).Document();
                var chat = new AdminChatDocument {  ProfileId = profileId,ProfileName = ProfileName,State = "pending", CreateAt = DateTime.UtcNow , Subject= Subject};
                await chatDoc.SetAsync(chat);
                chat.Id = chatDoc.Id;
                return chat;             
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public async Task<List<AdminChatDocument>?> GetAllChatWithAdmin(string profile1,DateTime? stardAfter)
        {
            if (string.IsNullOrEmpty(profile1)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName)
                    .Where(
                    Filter.And(Filter.EqualTo("ProfileId", profile1),Filter.NotEqualTo("State","closed"))
                    )
                    .OrderByDescending("CreateAt").Limit(10);
                if (stardAfter.HasValue)
                {
                    query.StartAfter(stardAfter.Value);
                }
                var list = new List<AdminChatDocument>();
                var snapshot = await query.GetSnapshotAsync();
                if (snapshot.Count != 0)
                {
                    foreach ( var chat in snapshot)
                    {
                        list.Add(chat.ConvertTo<AdminChatDocument>());
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public async Task<AdminChatDocument?> GetActiveChatWithAdmin(string profile1)
        {
            if (string.IsNullOrEmpty(profile1)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName)
                    .Where(Filter.And(Filter.EqualTo("ProfileId", profile1), Filter.EqualTo("State", "active")))
                    .Limit(1);
                
             
                var snapshot = await query.GetSnapshotAsync();
               
                return snapshot[0].ConvertTo<AdminChatDocument>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        } 
        public async Task<bool?> ActiveChatWithAdmin(string ChatId, string AdminID,string AdminFullName)
        {
            if (string.IsNullOrEmpty(ChatId) || string.IsNullOrEmpty(AdminID) || string.IsNullOrEmpty(AdminFullName)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(ChatId);
                await docRef.UpdateAsync(new Dictionary<string, object> { { "AdminName", AdminFullName }, { "AdminId", AdminID }, {"State","active" } });
                return true;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool?> CloseChatWithAdmin(string ChatId)
        {
            if (string.IsNullOrEmpty(ChatId)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(ChatId);
                await docRef.UpdateAsync(new Dictionary<string, object> { { "State", "closed" } });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<List<AdminChatDocument>?> GetAllChatsFilteredAsync(
             DateTime? startAfter,
             string? state = null,
             string? subject = null,
             string? profileId = null,
             string? profileName = null)
        {
            try
            {
                CollectionReference colRef = _firestoreDb.Collection(_collectionName);
                Query query = colRef;

                if (!string.IsNullOrWhiteSpace(state)) query = query.WhereEqualTo("State", state);

                if (!string.IsNullOrWhiteSpace(subject)) query = query.WhereEqualTo("Subject", subject);

                if (!string.IsNullOrWhiteSpace(profileId)) query = query.WhereEqualTo("ProfileId", profileId);

                if (!string.IsNullOrWhiteSpace(profileName)) query = query.WhereEqualTo("ProfileName", profileName);

                query = query.OrderByDescending("CreateAt").Limit(10);

                if (startAfter.HasValue)
                {
                    Timestamp firestoreTs = Timestamp.FromDateTime(startAfter.Value.ToUniversalTime());
                    query = query.StartAfter(firestoreTs);
                }

                var snapshot = await query.GetSnapshotAsync();

                var list = new List<AdminChatDocument>();
                foreach (var chat in snapshot)
                {
                    list.Add(chat.ConvertTo<AdminChatDocument>());
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firestore Error: {ex.Message}");
                return null;
            }
        }

    }
}
