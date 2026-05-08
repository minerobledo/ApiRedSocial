using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Event.Command.AcceptDeny
{
    public class AcceptDenyEventCommand : IRequest<Response<bool?>>
    {
        public string? Id { get; set; }
        public DateTime date { get; set; }
        public bool Status { get; set; }
    }
}
