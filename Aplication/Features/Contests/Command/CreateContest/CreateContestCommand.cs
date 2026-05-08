using Aplication.DTO.InputDto.Contest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Contests.Command.CreateContest
{
    public class CreateContestCommand: IRequest<Response<bool?>>
    {
        public ContestToCreate contestToCreate { get; set; }
    }
}
