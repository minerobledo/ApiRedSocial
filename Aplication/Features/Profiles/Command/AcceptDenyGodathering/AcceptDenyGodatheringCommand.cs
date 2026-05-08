using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.AcceptDenyGodathering
{
    public class AcceptDenyGodatheringCommand: IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? principal { get; set; } = null;
        public bool AcceptDenyGodathering { get; set; }
        public string? Id { get; set; }
    }
}
