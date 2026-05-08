using Domain.Entities;
using MediatR;
using Aplication.ResponPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.DTO.Profile;
using Aplication.DTO.Profile.GetProfileQuery;
using System.Security.Claims;
using Aplication.DTO.OutputDto.Profile;

namespace Aplication.Features.Profiles.Queries.GetProfiles
{
    public class GetProfilesLongQuery : IRequest<Response<ProfileLongDto>>
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public string? profileName { get; set; }

    }
}
