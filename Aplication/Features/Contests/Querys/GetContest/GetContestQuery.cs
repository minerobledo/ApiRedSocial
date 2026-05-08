using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Contests.Querys.GetContest
{
    public class GetContestQuery : IRequest<Response<List<Contest>?>>
    {
        public DateTime? DateTime { get; set; }
    }
}
