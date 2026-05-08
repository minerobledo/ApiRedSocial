using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.DTO.InputDto.Login.reponceProfileAndUser;
using Aplication.DTO.OutputDto.Profile;

namespace Aplication.Features.Profiles.Command.Login
{
    public class LoginCommand : IRequest<Response<LoginResponseDto>>
    {
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public DeviceTokenDto? DeviceToken { get; set; }
    }
}
