using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Reports.Command.ChangeStateReport
{
    public class ChangeStateReportCommand : IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? Principal { get; set; } = null;
        public string? Id { get; set; }
        public string? State { get; set; }
        public string? Result { get; set; } = null;
    }
}
