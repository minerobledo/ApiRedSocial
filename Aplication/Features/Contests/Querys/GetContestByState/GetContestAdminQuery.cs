using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Contests.Querys.GetContestByState
{ 
    public class GetContestAdminQuery : IRequest<Response<List<Contest>>>
    {
        
        public DateTime? StartAfter { get; set; }
    }
}
