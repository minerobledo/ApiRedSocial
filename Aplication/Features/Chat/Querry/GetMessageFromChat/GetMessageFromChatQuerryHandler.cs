using Aplication.Interfaces.Repository;
using Domain.Entities.Chats;
using Grpc.Net.Client.Balancer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Querry.GetMessageFromChat
{
    public class GetMessageFromChatQuerryHandler: IRequestHandler<GetMessageFromChatQuerry,Response<List<ChatMessage>?>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;

        public GetMessageFromChatQuerryHandler(IChatRepository chatRepository, IMessageRepository messageRepository)
        {
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
        }

        public async Task<Response<List<ChatMessage>?>> Handle(GetMessageFromChatQuerry request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.chatId)) return new Response<List<ChatMessage>?> { succeeded = true, data = null };
            try
            {
                 var a = new List<ChatMessage>();
                if (request.Type == "common")
                {
                    if (request.StartAfeter.HasValue)
                    {
                        var t = request.StartAfeter.Value;
                        a = await _messageRepository.GetMessagesByChatIdAsync(request.chatId,t);
                    }
                    else
                    {
                        a = await _messageRepository.GetMessagesByChatIdAsync(request.chatId);
                    }
                    return new Response<List<ChatMessage>?> { succeeded = true, data = a };
                }
                else if(request.Type == "admin")
                {
                    if (request.StartAfeter.HasValue)
                    {
                        var t = request.StartAfeter.Value;
                        a = await _messageRepository.GetAdminMessagesByChatIdAsync(request.chatId,t);
                    }
                    else
                    {
                        a = await _messageRepository.GetAdminMessagesByChatIdAsync(request.chatId);
                    }
                  
                    return new Response<List<ChatMessage>?> { succeeded = true, data = a };
                }
                return new Response<List<ChatMessage>?> { succeeded = true, data = a };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }

                return new Response<List<ChatMessage>?> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
