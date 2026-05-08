using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.SetTrustedDevice
{
    internal class SetTrustedDeviceCommandHandler: IRequestHandler<SetTrustedDeviceCommand, Response<bool?>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public SetTrustedDeviceCommandHandler(IProfileRepository profileRepository, IJwtTokenService jwtTokenService)
        {
            _profileRepository = profileRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<bool?>> Handle(SetTrustedDeviceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var selfId = _jwtTokenService.GetProfileIdFromJwt(request.principal);
                var a = await _profileRepository.SetTrustedDevice(request.DeviceId, request.Marca, request.Model, request.User, selfId);
                return new Response<bool?> { succeeded = true, data = a };
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new Response<bool?> { succeeded = false,errors = new List<Exception> { ex } };
            }


            throw new NotImplementedException();
        }
    }
}
