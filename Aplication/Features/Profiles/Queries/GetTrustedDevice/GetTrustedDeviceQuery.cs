using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Queries.GetTrustedDevice
{
    public class GetTrustedDeviceQuery: IRequest<Response<List<TrustedDevice>?>>
    {
        public ClaimsPrincipal? Principal { get; set; } = null;
        public int User {  get; set; }
    }
}
