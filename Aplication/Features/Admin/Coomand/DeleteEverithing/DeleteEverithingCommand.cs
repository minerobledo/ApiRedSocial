using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Coomand.DeleteEverithing
{
    public class DeleteEverithingCommand: IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? principal {  get; set; } = null;
        public string? Type { get; set; }
        public string? Id { get; set; }
    }
}
