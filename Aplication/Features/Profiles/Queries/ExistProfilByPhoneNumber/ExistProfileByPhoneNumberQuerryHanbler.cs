using Aplication.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Queries.ExistProfilByPhoneNumber
{
    public class ExistProfileByPhoneNumberQuerryHanbler : IRequestHandler<ExistProfileByPhoneNumberQuerry, Response<bool?>>
    {
        private readonly IProfileRepository _profileRepository;
        public ExistProfileByPhoneNumberQuerryHanbler(IProfileRepository profileRepository )
        {
            _profileRepository = profileRepository;
        }

        public async Task<Response<bool?>> Handle(ExistProfileByPhoneNumberQuerry request, CancellationToken cancellationToken)
        {
            try
            {
                var flag = await _profileRepository.ExistProfileByPhoneAsync(request.PhoneToCheck!);
                if (flag == null) return new Response<bool?> { succeeded = false, message = "Error en la bace de datos", data = flag };
                else return new Response<bool?> { succeeded = true, data = flag };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new Response<bool?> { succeeded = false, message = "Error en la api ", data = null };
            }

        }
    }
}
