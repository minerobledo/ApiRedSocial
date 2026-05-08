using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using AutoMapper;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.UpdateGeoPoint
{
    public class UpdateGeoPoinCommandHandler: IRequestHandler<UpdateGeoPoinCommand, Response<bool?>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public UpdateGeoPoinCommandHandler(IProfileRepository profileRepository, IJwtTokenService jwtTokenService)
        {
            _profileRepository = profileRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<bool?>> Handle(UpdateGeoPoinCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var selfId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
                var geo = new GeoPoint(request.Lat,request.Long);
                var a = await _profileRepository.UpdateGeoPoint(geo, selfId, request.user);
                return new Response<bool?>() { succeeded = true, data = a };
            }
            catch (Exception ex)
            {
                return new Response<bool?>() { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
