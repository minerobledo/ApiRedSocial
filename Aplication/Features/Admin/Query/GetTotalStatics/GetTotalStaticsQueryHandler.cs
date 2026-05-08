using Aplication.Interfaces.Repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Query.GetTotalStatics
{
    internal class GetTotalStaticsQueryHandler: IRequestHandler<GetTotalStaticsQuery,Response<TotalStatics>>
    {
        private readonly IStatisticsRepocitory _statisticsRepocitory;

        public GetTotalStaticsQueryHandler(IStatisticsRepocitory statisticsRepocitory)
        {
            _statisticsRepocitory = statisticsRepocitory;
        }

        public async Task<Response<TotalStatics>> Handle(GetTotalStaticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var a = await _statisticsRepocitory.GetTotalStatics();
                return new Response<TotalStatics> { succeeded = true, data = a };
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
                return new Response<TotalStatics> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
