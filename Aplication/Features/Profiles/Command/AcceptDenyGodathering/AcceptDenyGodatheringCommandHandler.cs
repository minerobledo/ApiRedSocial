using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.AcceptDenyGodathering
{
    internal class AcceptDenyGodatheringCommandHandler : IRequestHandler<AcceptDenyGodatheringCommand, Response<bool?>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;


        public async Task<Response<bool?>> Handle(AcceptDenyGodatheringCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var selfId = _jwtTokenService.GetProfileIdFromJwt(request.principal);
                var selfPrifile = await _profileRepository.GetProfileByIdAsync(selfId);

                var a = await _profileRepository.UpdateGodFatherResponce(request.Id, request.AcceptDenyGodathering);
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
                return new Response<bool?> { succeeded = false, errors = new List<Exception> {ex } };
            }



            throw new NotImplementedException();
        }
    }
}
