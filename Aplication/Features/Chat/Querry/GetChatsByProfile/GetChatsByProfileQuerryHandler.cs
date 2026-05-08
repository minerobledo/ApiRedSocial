using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities.Chats;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Querry.GetChatsByProfile
{
    internal class GetChatsByProfileQuerryHandler: IRequestHandler<GetChatsByProfileQuerry, Response<List<ChatDocument>>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public GetChatsByProfileQuerryHandler(IChatRepository chatRepository, IJwtTokenService jwtTokenService)
        {
            _chatRepository = chatRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<List<ChatDocument>>> Handle(GetChatsByProfileQuerry request, CancellationToken cancellationToken)
        {
            if(request.principal == null) return new Response<List<ChatDocument>> { succeeded = true, data = null };
            var selfId = _jwtTokenService.GetProfileIdFromJwt(request.principal);
            if (selfId == null) return new Response<List<ChatDocument>> { succeeded = true, data = null };

            try
            {
                var list = await _chatRepository.GetChatsByUserIdAsync(selfId);
                return new Response<List<ChatDocument>> {succeeded = true, data = list };
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
                return new Response<List<ChatDocument>> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
