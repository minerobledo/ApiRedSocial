using Aplication.DTO.OutputDto.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.SetTrustedDevice
{
    public class SetTrustedDeviceCommand : IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? principal { get; set; } = null;
        public string? DeviceId { get; set; }
        public string? Marca {  get; set; }
        public string? Model { get; set; }
        public int User { get; set; }
    }
}
