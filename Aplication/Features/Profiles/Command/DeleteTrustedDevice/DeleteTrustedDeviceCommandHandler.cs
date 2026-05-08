using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.DeleteTrustedDevice
{
    internal class DeleteTrustedDeviceCommandHandler:IRequestHandler<DeleteTrustedDeviceCommand, Response<bool?>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public DeleteTrustedDeviceCommandHandler(IProfileRepository profileRepository, IJwtTokenService jwtTokenService)
        {
            _profileRepository = profileRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<bool?>> Handle(DeleteTrustedDeviceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var selfId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
                var a = await _profileRepository.DeleteTrustedDevice(request.DocumentID, selfId, request.User);
                return new Response<bool?> { succeeded = true, data = a };

            }catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new Response<bool?> { succeeded = false, errors = new List<Exception> { ex } };
            }
           
        }
    }
}
