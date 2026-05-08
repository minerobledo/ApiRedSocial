using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Aplication.ResponPattern;
using Aplication.Interfaces.Repository;

namespace Aplication.Features.Profiles.Queries.EsistTokenGodfather
{
    public class ExistTokenGodfatherQueryHandler : IRequestHandler<ExistTokenGodfatherQuery, Response<bool>>
    {
        private readonly IProfileRepository _profileRepository;
        public ExistTokenGodfatherQueryHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<Response<bool>> Handle(ExistTokenGodfatherQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (await _profileRepository.ExistProfileByTokenGodfather(request.token!)) return new Response<bool>()
                {
                    succeeded = true,
                    data = true
                };
                return new Response<bool>()
                {
                    succeeded = false,
                    data = false
                };

            }
            catch (Exception ex)
            {
                return new Response<bool>()
                {
                    succeeded= false,
                    data = false,
                    errors = new List<Exception> { ex }

                };
            }
           
        }
    }
}
