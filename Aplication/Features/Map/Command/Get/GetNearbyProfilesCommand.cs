using Aplication.DTO.OutputDto.Profile;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Map.Command.Get
{
    public class GetNearbyProfilesCommand : IRequest<Response<List<ProfileShortDto>>>
    {
        public ClaimsPrincipal? Principal { get; set; } = null;
        public Dictionary<string, object>? Filter { get; set; }
        public double lat { get;set; }
        public double lng { get; set; }
        public double Radius { get; set; }
    }
}
