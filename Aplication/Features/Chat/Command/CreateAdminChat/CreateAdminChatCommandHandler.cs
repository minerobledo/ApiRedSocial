using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Command.CreateAdminChat
{
    internal class CreateAdminChatCommandHandler: IRequestHandler<CreateAdminChatCommand, Response<AdminChatDocument?>>
    {
        
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAdminChatRepository _adminChatRepository;

        public CreateAdminChatCommandHandler(IJwtTokenService jwtTokenService, IAdminChatRepository adminChatRepository)
        {
            
            _jwtTokenService = jwtTokenService;
            _adminChatRepository = adminChatRepository;
        }

        public async Task<Response<AdminChatDocument?>> Handle(CreateAdminChatCommand request, CancellationToken cancellationToken)
        {
            if (request.Principal == null || string.IsNullOrEmpty(request.SelfProfileName)) return new Response<AdminChatDocument?> { succeeded = true, data = null };
            var selfProfileId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
            try
            {
                var a = await _adminChatRepository.CreateAdminChatAsync(selfProfileId!, request.SelfProfileName!, request.Subject!);
                if (a != null)
                {
                    return new Response<AdminChatDocument?> { succeeded = true, data = a };
                }
                return new Response<AdminChatDocument?> { succeeded = true, data = a };
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
                return new Response<AdminChatDocument?> { succeeded = false, errors = new List<Exception> { ex } };
            }
            throw new NotImplementedException();
        }
    }

}
