using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Queries.SerchProfile
{
    public class SerchProfileQueryHandler : IRequestHandler<SerchProfileQuery, Response<List<ProfileShortDto>>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public SerchProfileQueryHandler(IJwtTokenService jwtTokenService, IProfileRepository profileRepository,IMapper mapper)
        {
            _jwtTokenService = jwtTokenService;
            _profileRepository = profileRepository;
            
            _mapper = mapper;
        }

        public async Task<Response<List<ProfileShortDto>>> Handle(SerchProfileQuery request, CancellationToken cancellationToken)
        {
            var profileId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
            if(profileId == null) return new Response<List<ProfileShortDto>>() { succeeded = true,data = null };

            var listProfile =await _profileRepository.SerchProfile(request.Name!.ToLower());
            if (listProfile == null) return new Response<List<ProfileShortDto>>() { succeeded = true, data = { } };

            List<ProfileShortDto> list = new List<ProfileShortDto>();
            try
            {
                foreach (var item in listProfile)
                {
                    if (item.Id  != profileId)
                    {
                        var a = _mapper.Map<ProfileShortDto>(item);
                        list.Add(a);
                    }
                }
                return new Response<List<ProfileShortDto>>() {succeeded = true, data = list };
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<List<ProfileShortDto>>() { succeeded = false, data = null };
            }
        }
    }
}
