using Aplication.Interfaces.Repository;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Command.CloseAdminChat
{
    internal class CloseAdminChatCommandHandler: IRequestHandler<CloseAdminChatCommand, Response<bool?>>
    {
        private readonly IAdminChatRepository _adminChatRepository;

        public CloseAdminChatCommandHandler(IAdminChatRepository adminChatRepository)
        {
            _adminChatRepository = adminChatRepository;
        }

        public async Task<Response<bool?>> Handle(CloseAdminChatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var a = await _adminChatRepository.CloseChatWithAdmin(request.ChatId!);
                return new Response<bool?> { succeeded = true, data = a };
            }catch (Exception ex)
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
                return new Response<bool?> { succeeded = false, errors = new List<Exception> { } };
            }
        }
    }
}
