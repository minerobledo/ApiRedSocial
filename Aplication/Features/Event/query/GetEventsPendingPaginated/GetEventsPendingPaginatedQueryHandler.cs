using Aplication.Features.Event.query.GetEventsAceptedPaginated;
using Aplication.Interfaces.Repository;
using Domain.Entities.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Event.query.GetEventsPendingPaginated
{
    public class GetEventsPendingPaginatedQueryHandler: IRequestHandler<GetEventsPendingPaginatedQuery, Response<List<EventEntity>?>>
    {

        private readonly IEventRepocitory _eventRepocitory;

        public GetEventsPendingPaginatedQueryHandler(IEventRepocitory eventRepocitory)
        {
            _eventRepocitory = eventRepocitory;
        }

        

        public async Task<Response<List<EventEntity>?>> Handle(GetEventsPendingPaginatedQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return new Response<List<EventEntity>?> { succeeded = true, data = await _eventRepocitory.GetEventsPendingPaginated(request.Date) };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }

                return new Response<List<EventEntity>?> { succeeded = true, errors = new List<Exception> { ex } };
            }
           
        }
       
    }
}
