using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Queries.ExistProfilByPhoneNumber
{
    public class ExistProfileByPhoneNumberQuerry : IRequest<Response<bool?>>
    {
        public string? PhoneToCheck { get; set; }
    }
}
