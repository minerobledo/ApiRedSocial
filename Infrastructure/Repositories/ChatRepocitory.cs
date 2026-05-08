using Aplication.Interfaces.Repository;
using Domain.Entities.Chats;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal class ChatRepocitory: GenericRepository<ChatDocument>, IChatRepository
    {
        

        public ChatRepocitory(FirestoreDb firestoreDb, string collectionName = "Chat") : base(firestoreDb, collectionName)
        {
           
        }

        public async Task<ChatDocument?> CreateChatAsync(string profile1, string profile2, string profile1Name, string profile2Name)
        {
            if (string.IsNullOrEmpty(profile1)|| string.IsNullOrEmpty(profile2)) return null;
            try
            {
                var chatDoc = _firestoreDb.Collection(_collectionName).Document();
                var chat = new ChatDocument { Profile1 = profile1, Profile2 = profile2,Profile1Name = profile1Name, Profile2Name = profile2Name, CreateAt = DateTime.UtcNow };
                await chatDoc.SetAsync(chat);
                chat.Id = chatDoc.Id; 
                return chat;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public async Task<ChatDocument?> getChatbByTwoProfil(string profile1, string profile2)
        {
            if (string.IsNullOrEmpty(profile1) || string.IsNullOrEmpty(profile2)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName).Where
                    (
                        Filter.Or
                        (
                            Filter.And
                            (
                                Filter.EqualTo("Profile1", profile1),
                                Filter.EqualTo("Profile2", profile2)
                            ),
                             Filter.And
                            (
                                Filter.EqualTo("Profile1", profile2),
                                Filter.EqualTo("Profile2", profile1)
                            )
                        )
                    ).Limit(1);
                var snapshot = await query.GetSnapshotAsync();
                if (snapshot.Count == 0) return null;
                return snapshot[0].ConvertTo<ChatDocument>();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
}
        public async Task<List<ChatDocument>?> GetChatsByUserIdAsync(string profileId)
        {

            if (string.IsNullOrEmpty(profileId)) return null;
            try
            {
                var query = _firestoreDb.Collection(_collectionName).
                    Where
                    (
                        Filter.Or
                        (
                            Filter.EqualTo("Profile1", profileId),
                            Filter.EqualTo("Profile2", profileId)
                        )
                    );
                var snapshot = await query.GetSnapshotAsync();
                var list = new List<ChatDocument>();
                foreach (var item in snapshot)
                {
                    list.Add(item.ConvertTo<ChatDocument>());
                }
                return list;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
                
        }
        
    }
}
