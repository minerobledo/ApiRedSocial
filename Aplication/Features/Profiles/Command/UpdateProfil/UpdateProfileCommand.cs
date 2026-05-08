using Aplication.DTO.InputDto.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Aplication.DTO.OutputDto.Profile;



namespace Aplication.Features.Profiles.Command.UpdateProfil
{
    public class UpdateProfileCommand: IRequest<Response<SelfProfile?>>
    {
        public ClaimsPrincipal Principal { get; set; }
        public ProfileEditDto profileEdit { get; set; }
    }
}
