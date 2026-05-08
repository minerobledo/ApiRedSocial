using Domain.Entities.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Event.Command.UpdateEvent
{
    public class UpdateEventCommand : IRequest<Response<bool?>>
    {
        public string? Id {  get; set; }
        public Dictionary<string ,object>? Parameters { get; set; }
    }
}
