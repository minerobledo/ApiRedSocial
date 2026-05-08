using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Contests.Command.EdutContest
{
    public class EditContestCommand : IRequest<Response<bool?>>
    {
         public Contest Contest { get; set; }
    }
}
