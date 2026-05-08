using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using BCrypt.Net;
using Domain.Entities.Chats;
using FirebaseAdmin.Messaging;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    internal class MessageRepository :GenericRepository<ChatMessage>, IMessageRepository
    {
        private readonly string _superColectionName= "Chat";
        private readonly string _adminColectionName = "AdminChat";
        
       
        public MessageRepository( FirestoreDb firestoreDb, string collectionName = "Messages") : base(firestoreDb, collectionName)
        {
        }

        public async Task<bool?> SaveMessageAsync(ChatMessage message ,string chatId)
        {
            if (message == null || string.IsNullOrEmpty(chatId)) return null;             
            try
            {
                
                await _firestoreDb.Collection(_superColectionName)
                    .Document(chatId)
                    .Collection(_collectionName)
                    .AddAsync(message);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<ChatMessage>?> GetMessagesByChatIdAsync(string chatId, DateTime? startAfter = null)
        {
            if (string.IsNullOrEmpty(chatId)) return null;

            try
            {
                var query = _firestoreDb.Collection(_superColectionName)
                    .Document(chatId)
                    .Collection(_collectionName)
                    .OrderByDescending("Timestamp")
                    .Limit(30);

                if (startAfter != null)
                {
                    query = query.StartAfter(startAfter);
                }

                var snapshot = await query.GetSnapshotAsync();
                var list = new List<ChatMessage>();

                foreach (var item in snapshot)
                {
                    list.Add(item.ConvertTo<ChatMessage>());
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<bool?> SaveAdminMessageAsync(ChatMessage message, string chatId)
        {
            if (message == null || string.IsNullOrEmpty(chatId)) return null;
            try
            {
                await _firestoreDb.Collection(_adminColectionName)
                    .Document(chatId)
                    .Collection(_collectionName)
                    .AddAsync(message);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<ChatMessage>?> GetAdminMessagesByChatIdAsync(string chatId,DateTime? startAfter = null)
        {
            if (string.IsNullOrEmpty(chatId)) return null;
            try
            {
                var query = _firestoreDb.Collection(_adminColectionName)
                    .Document(chatId)
                    .Collection(_collectionName)
                    .OrderByDescending("Timestamp").Limit(30);

                if (startAfter != null)
                {
                    query = query.StartAfter(startAfter);
                }

                var snapshot = await query.GetSnapshotAsync();

                var list = new List<ChatMessage>();

                foreach (var item in snapshot)
                {
                    list.Add(item.ConvertTo<ChatMessage>());
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        
    }
}
