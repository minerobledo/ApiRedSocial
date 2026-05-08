using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Aplication.ResponPattern;
using System.Security.Claims;
using Aplication.DTO.OutputDto.Profile;

namespace Aplication.Features.Profiles.Queries.SerchProfile
{
    public class SerchProfileQuery : IRequest<Response<List<ProfileShortDto>>>
    {
        public ClaimsPrincipal? Principal { get; set; }
        public string? Name { get; set; }
        
    }
}
