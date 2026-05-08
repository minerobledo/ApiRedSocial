using Aplication.Interfaces.Repository;
using Aplication.ResponPattern;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Queries.ExistProfileName
{
    public class ExistProfileNameQueryHandler : IRequestHandler<ExistProfileNameQuery,Response<bool?>>
    {
        private readonly IProfileRepository _profileRepository;
        public ExistProfileNameQueryHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<Response<bool?>> Handle(ExistProfileNameQuery request, CancellationToken cancellationToken)
        {
            try
            {

                return new Response<bool?>
                {
                    succeeded = true,
                    data = await _profileRepository.ExistProfileByNameProfileAsync(request.ProfileNameToCheck)
                };


            }
            catch (Exception ex)
            {
                return new Response<bool?>
                {
                    succeeded = false,
                    errors = new List<Exception> { ex }
                };

            }
        }
    }
}
