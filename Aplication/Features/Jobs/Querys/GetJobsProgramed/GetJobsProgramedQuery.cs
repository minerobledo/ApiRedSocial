using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Jobs.Querys.GetJobsProgramed
{
    public class GetJobsProgramedQuery: IRequest<Response<List<ITrigger>>>
    {
    }
}
