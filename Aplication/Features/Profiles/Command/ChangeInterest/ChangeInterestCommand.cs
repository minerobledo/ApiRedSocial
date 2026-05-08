using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.ChangeInterest
{
    public class ChangeInterestCommand : IRequest<Response<bool?>>
    {
        public ClaimsPrincipal principal {  get; set; }
        public string Interest {  get; set; }
    }
}
