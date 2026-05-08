using Domain.Entities.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Event.query.GetEventsPendingPaginated
{
    public class GetEventsPendingPaginatedQuery : IRequest<Response<List<EventEntity>?>>
    {
        public DateTime? Date { get; set; }
    }
}
