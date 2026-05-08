using Aplication.Interfaces.Repository;
using Domain.Entities.Chats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Chat.Querry.GetChatsBystateForAdmin
{
    internal class GetChatsByStateQuerryHandler: IRequestHandler< GetChatsByStateQuerry, Response<List<AdminChatDocument>>>
    {
        private readonly IAdminChatRepository _adminChatRepository;

        public GetChatsByStateQuerryHandler(IAdminChatRepository adminChatRepository)
        {
            _adminChatRepository = adminChatRepository;
        }

        public async Task<Response<List<AdminChatDocument>>> Handle(GetChatsByStateQuerry request, CancellationToken cancellationToken)
        {
            try
            {
                return new Response<List<AdminChatDocument>> { succeeded = true, data = await _adminChatRepository.GetAllChatsFilteredAsync(request.StartAfter, request.State, request.Subject,request.ProfileId,request.ProfileName) };
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
