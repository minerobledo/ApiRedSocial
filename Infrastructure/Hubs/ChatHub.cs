using Aplication.Interfaces.Repository;
using Domain.Entities.Chats;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private static readonly Dictionary<string, string> _connections = new();
        public ChatHub(IProfileRepository profileRepository, IFirebaseMessagingRepository firebaseMessagingRepository, IChatRepository chatRepository, IMessageRepository messageRepository)
        {
            Console.WriteLine("ACA: se crea el objeto ChatHub");
            _profileRepository = profileRepository;
            _chatRepository = chatRepository;
            _firebaseMessagingRepository = firebaseMessagingRepository;
            _messageRepository = messageRepository;

        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var forwardedHost = httpContext?.Request.Headers["X-Forwarded-Host"].ToString();
            var host = httpContext?.Request.Headers["Host"].ToString();

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var user = _connections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (!string.IsNullOrEmpty(user))
            {
                Console.WriteLine("ACA: se desconecta");
                _connections.Remove(user);
                Console.WriteLine($"❌ Usuario desconectado: {user}");
            }

            return base.OnDisconnectedAsync(exception);
        }       

        public async Task SendMessage(string senderId, string receiverId, string message, string userNameSender, string chatId, string IV)
        {
            Console.WriteLine("ACA: entra en mandar mensajes");
            var devis = await _profileRepository.GetDeviceTokenAsync(receiverId);
            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                UserNameSender = userNameSender,
                Message = message,
                Timestamp = DateTime.UtcNow,
                IV = IV,
                IsRead = false
            };
            foreach (var item in devis)
            {
                Console.WriteLine("ACA: se registran los dispositivos para mensajear");
                _firebaseMessagingRepository.SendNotificationAsync(item.Token);
            }


            var a = await _messageRepository.SaveMessageAsync(chatMessage, chatId);
            
            // Notificar al receptor si está conectado
            await Clients.Group(chatId).SendAsync("ReceiveMessage", chatMessage);
            Console.WriteLine("ACA: se manda el mensaje");
        }
        public async Task SendAdminMessage(string senderId, string receiverId, string message, string userNameSender, string chatId, string IV)
        {
            Console.WriteLine("ACA: entra en mandar mensajes");
            var devis = await _profileRepository.GetDeviceTokenAsync(receiverId);
            var chatMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                UserNameSender = userNameSender,
                Message = message,
                Timestamp = DateTime.UtcNow,
                IV = IV,
                IsRead = false
            };
            if (devis != null)
            {
                foreach (var item in devis)
                {
                    Console.WriteLine("ACA: se registran los dispositivos para mensajear");
                    _firebaseMessagingRepository.SendNotificationAsync(item.Token);
                }
            }



            var a = await _messageRepository.SaveAdminMessageAsync(chatMessage, chatId);
            
            // Notificar al receptor si está conectado
            await Clients.Group(chatId).SendAsync("ReceiveMessage", chatMessage);
            Console.WriteLine("ACA: se manda el mensaje");
        }
        public async Task JoinChat(string chatId)
        {
            Console.WriteLine("ACA: se une al chat");
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);         
        }
    }
}

