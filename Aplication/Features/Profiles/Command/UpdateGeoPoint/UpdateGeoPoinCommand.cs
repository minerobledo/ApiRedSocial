using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.UpdateGeoPoint
{
    public class UpdateGeoPoinCommand : IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? Principal { get; set; } = null;

        public double Lat {  get; set; }
        public double Long { get; set; }
        public int user {  get; set; }
    }
}
