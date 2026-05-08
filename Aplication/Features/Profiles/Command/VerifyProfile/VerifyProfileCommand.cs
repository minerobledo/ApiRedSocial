using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.VerifyProfile
{
    public class VerifyProfileCommand : IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? Principal { get; set; } = null;

        public string? Id { get; set; }

    }
}
