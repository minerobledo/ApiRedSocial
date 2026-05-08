using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Reports.Command.ReportProfile
{
    public class ReportProfileCommand : IRequest<Response<bool?>>
    {
        public string? Type { get; set; }
        public ClaimsPrincipal? Principal { get; set; } = null;
        public string? ReporterProfileName { get; set; }
        public string? ReportedProfileId { get; set; }
        public string? ReportedProfileName { get; set; }
    }
}
