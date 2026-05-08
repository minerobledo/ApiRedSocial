using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.DTO.InputDto.Login.reponceProfileAndUser;
using Aplication.DTO.OutputDto.Profile;


namespace Aplication.Features.Profiles.Command.TokenLogin
{
    public class TokenLoginCommand: IRequest<Response<LoginResponseDto>>
    {
        public string? Device { get; set; }
        public string? Token { get; set; }
        public DeviceTokenDto? DeviceToken { get; set; }
    }
}
