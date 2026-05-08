using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Queries.GetTrustedDevice
{
    internal class GetTrustedDeviceQueryHandler: IRequestHandler<GetTrustedDeviceQuery, Response<List<TrustedDevice>?>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IProfileRepository _profileRepository;

        public GetTrustedDeviceQueryHandler(IJwtTokenService jwtTokenService, IProfileRepository profileRepository)
        {
            _jwtTokenService = jwtTokenService;
            _profileRepository = profileRepository;
        }

        public async Task<Response<List<TrustedDevice>?>> Handle(GetTrustedDeviceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var selId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
                var a = await _profileRepository.GetAllTrustedDevice(selId, request.User);
                return new Response<List<TrustedDevice>?>() { succeeded = true, data = a };
            }
            catch (Exception ex)
            {
                return new Response<List<TrustedDevice>?>() { succeeded = false, errors = new List<Exception> { ex } };
            }
            throw new NotImplementedException();
        }
    }
}
