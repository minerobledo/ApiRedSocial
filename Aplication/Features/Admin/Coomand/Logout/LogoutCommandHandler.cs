using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.Logout
{
    internal class LogoutAdminCommandHandler: IRequestHandler<LogoutAdminCommand,Response<bool?>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IProfileRepository _profileRepository;
        private readonly IRefresTokenRepository _refresTokenRepository;

        public LogoutAdminCommandHandler(IJwtTokenService jwtTokenService, IProfileRepository profileRepository, IRefresTokenRepository refresTokenRepository)
        {
            _jwtTokenService = jwtTokenService;
            _profileRepository = profileRepository;
            _refresTokenRepository = refresTokenRepository;
        }

        public async Task<Response<bool?>> Handle(LogoutAdminCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool? result = null;
                var selfId = _jwtTokenService.GetAdminIdFromJwt(request.Principal!);
                if (selfId != null)
                {
                    var profile= await _profileRepository.GetProfileByIdAsync(selfId);
                    if (profile != null)
                    {
                        var a = await _refresTokenRepository.GetRefresTokenDocumentIdIfExist(selfId,null);
                        if (a != null)
                        {
                            await _refresTokenRepository.DeleteRefeshtoken(a);
                        }
                        result = true;
                    }
                }
                return new Response<bool?> { data= result, succeeded = true };
            }catch(Exception ex)
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
                return new Response<bool?> { succeeded = false, errors = new List<Exception> { ex } };
            }
            
        }
    }
}
