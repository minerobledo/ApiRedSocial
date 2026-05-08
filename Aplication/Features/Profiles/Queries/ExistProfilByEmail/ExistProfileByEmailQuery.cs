using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Queries.ExistProfilByEmail
{
    public class ExistProfileByEmailQuery : IRequest<Response<bool?>>
    {
        public string? EmailToCheck { get; set; }

    }
}
