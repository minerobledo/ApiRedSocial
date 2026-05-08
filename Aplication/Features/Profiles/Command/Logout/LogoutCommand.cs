using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.Logout
{
    public class LogoutCommand : IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? Principal { get; set; }
        public DeviceToken? DeviceToken { get; set; }
         public int User {  get; set; } 
    }
}
