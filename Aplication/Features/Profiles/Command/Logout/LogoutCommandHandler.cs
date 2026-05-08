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
    internal class LogoutCommandHandler: IRequestHandler<LogoutCommand,Response<bool?>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefresTokenRepository _refresTokenRepository;
        private readonly IProfileRepository _profileRepository;

        public LogoutCommandHandler(IJwtTokenService jwtTokenService, IRefresTokenRepository refresTokenRepository, IProfileRepository profileRepository)
        {
            _jwtTokenService = jwtTokenService;
            _refresTokenRepository = refresTokenRepository;
            _profileRepository = profileRepository;
        }

        public async Task<Response<bool?>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool? result = null;
                var selfId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
                var profile = await _profileRepository.GetProfileByIdAsync(selfId);
                if (profile != null)
                {
                    var a = await _refresTokenRepository.GetRefresTokenDocumentIdIfExist(selfId,request.User);
                    await _refresTokenRepository.DeleteRefeshtoken(a);
                    await _profileRepository.RemuveDeviceTokenAsync(profile, request.User, request.DeviceToken);

                    result = true;
                }
                return new Response<bool?> { data= result, succeeded = true };
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<bool?> { succeeded = false, errors = new List<Exception> { ex } };
            }
            
        }
    }
}
