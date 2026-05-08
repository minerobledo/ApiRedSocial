using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Reports.Query.GetReportByFilter
{
    public class GetReportByFilterQuerry: IRequest<Response<List<Report>>>
    {
        public Dictionary<string,object>? Filter { get; set; }
        public DateTime? StartAfter { get; set; } = null;
    }
}
