using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.DeleteProfile
{
    public class DeleteProfileCommand: IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? Principal { get; set; } = null;
        public string? id { get; set; }
    }
}
