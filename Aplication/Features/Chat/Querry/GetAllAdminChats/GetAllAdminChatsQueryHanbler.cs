using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Querry.GetAllAdminChats
{
    internal class GetAllAdminChatsQueryHanbler: IRequestHandler<GetAllAdminChatsQuery, Response<List<AdminChatDocument>>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAdminChatRepository _adminChatRepository;

        public GetAllAdminChatsQueryHanbler(IJwtTokenService jwtTokenService, IAdminChatRepository adminChatRepository)
        {
            _jwtTokenService = jwtTokenService;
            _adminChatRepository = adminChatRepository;
        }

        public async Task<Response<List<AdminChatDocument>>> Handle(GetAllAdminChatsQuery request, CancellationToken cancellationToken)
        {
            if (request.Principal == null) return new Response<List<AdminChatDocument>> { succeeded = true, data = new List<AdminChatDocument>() };

            try
            {
                var ProfileId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
                var a = await _adminChatRepository.GetAllChatWithAdmin(ProfileId, request.StartAfter);
                return new Response<List<AdminChatDocument>> { succeeded = true, data = a };

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
                return new Response<List<AdminChatDocument>> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
