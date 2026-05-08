using Aplication.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.test.Command
{
    public class GetProvinceCommandHandler //: IRequestHandler<GetProvinceCommand, List<Dictionary<string, object>>>
    {
        private readonly IProfileRepository _profileRepository;
        public GetProvinceCommandHandler(IProfileRepository profileRepository) 
        {
            _profileRepository = profileRepository;
        }

        //public Task<List<Dictionary<string, object>>> Handle(GetProvinceCommand request, CancellationToken cancellationToken)
        //{
        //    //return _profileRepository.ObtenerPerfilesPorNombreYProvinciaAsync("Chaco");
        //}

    }
}
