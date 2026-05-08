using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Queries.GetProfilesThatVerify
{
    internal class GetProfilesThatVerifyQueryHandler: IRequestHandler<GetProfilesThatVerifyQuery,Response<List<ProfileShortDto?>>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public GetProfilesThatVerifyQueryHandler(IProfileRepository profileRepository, IJwtTokenService jwtTokenService)
        {
            _profileRepository = profileRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<List<ProfileShortDto?>>> Handle(GetProfilesThatVerifyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _profileRepository.GetProfileByIdAsync(request.Id);
                List<ProfileShortDto> a = new List<ProfileShortDto>();
                if (profile!=null && profile.ListProfileAuthenticate != null)
                {
                    a = await _profileRepository.GetProfileShortListByListId(profile.ListProfileAuthenticate) ?? new List<ProfileShortDto>(); ;

                }
                return new Response<List<ProfileShortDto?>> { succeeded = true, data = a };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<List<ProfileShortDto?>> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
