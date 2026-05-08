using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.DeleteTrustedDevice
{
    public class DeleteTrustedDeviceCommand: IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? Principal { get; set; } = null;
        public int User { get; set; }
        public string? DocumentID { get; set; }
    }
}
