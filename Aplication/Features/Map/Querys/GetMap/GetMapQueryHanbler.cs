using Aplication.Interfaces.Repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Map.Querys.GetMap
{
    internal class GetMapQueryHanbler : IRequestHandler<GetMapQuery, Response<UsersByProvince?>>
    {
        private readonly IStatisticsRepocitory _statisticsRepocitory;

        public GetMapQueryHanbler(IStatisticsRepocitory statisticsRepocitory)
        {
            _statisticsRepocitory = statisticsRepocitory;
        }

        public async Task<Response<UsersByProvince?>> Handle(GetMapQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var a = await _statisticsRepocitory.GetUsersByProvince();

                return new Response<UsersByProvince?> { succeeded = true, data = a  };
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

                return new Response<UsersByProvince?> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
