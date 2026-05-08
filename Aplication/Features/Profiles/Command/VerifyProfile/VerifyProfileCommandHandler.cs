using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using AutoMapper.Configuration.Annotations;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.VerifyProfile
{
    internal class VerifyProfileCommandHandler:IRequestHandler<VerifyProfileCommand,Response<bool?>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public VerifyProfileCommandHandler(IProfileRepository profileRepository, IJwtTokenService jwtTokenService)
        {
            _profileRepository = profileRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<bool?>> Handle(VerifyProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool isAdmin = false;
                var selfId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
                Profile? profile;
                if (selfId == null)
                {
                    isAdmin = true;
                    selfId = _jwtTokenService.GetAdminIdFromJwt(request.Principal);
                    var a = await _profileRepository.VerifyProfile(selfId, request.Id,true);
                    return new Response<bool?> { succeeded = true, data = a };
                }
                if (!isAdmin)
                {
                    profile = await _profileRepository.GetProfileByIdAsync(selfId);
                    if(profile.NumberPersonAuthenticate >= 5)
                    {
                        var a = await _profileRepository.VerifyProfile(selfId, request.Id, false);
                        return new Response<bool?> { succeeded = true, data = a };
                    }
                    return new Response<bool?> { succeeded = true, data = false };
                }
                return new Response<bool?> { succeeded = false, data = null };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<bool?> { succeeded =false , errors = new List<Exception> { ex } };
            }
        }
    }
}
