using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Contests.Command.DeleteContest
{
    public class DeleteContestCommand: IRequest<Response<bool?>>
    {
        public string id {  get; set; }
    }
}
