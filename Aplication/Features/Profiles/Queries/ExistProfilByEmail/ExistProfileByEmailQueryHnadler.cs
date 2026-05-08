using Aplication.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Queries.ExistProfilByEmail
{
    public class ExistProfileByEmailQueryHnadler : IRequestHandler<ExistProfileByEmailQuery, Response<bool?>>
    {
        private readonly IProfileRepository _profileRepository1;
        public ExistProfileByEmailQueryHnadler(IProfileRepository profileRepository)
        {
            _profileRepository1 = profileRepository;

        }

        public async Task<Response<bool?>> Handle(ExistProfileByEmailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var flag = await _profileRepository1.ExistProfileByEmailAsync(request.EmailToCheck!);
                if (flag == null) return new Response<bool?> { succeeded = false, message = "Error en la bace de datos", data = flag};
                else return new Response<bool?> { succeeded = true, data = flag };
            }catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new Response<bool?> { succeeded = false, message = "Error en la api ", data = null };
            }
        }
    }
}
