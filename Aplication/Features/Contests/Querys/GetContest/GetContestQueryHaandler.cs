using Aplication.Interfaces.Repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Contests.Querys.GetContest
{
    internal class GetContestQueryHaandler : IRequestHandler<GetContestQuery, Response<List<Contest>?>>
    {
        private readonly IContestRespository _contestRespository;

        public GetContestQueryHaandler(IContestRespository contestRespository)
        {
            _contestRespository = contestRespository;
        }

        public async Task<Response<List<Contest>?>> Handle(GetContestQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List<Contest>? a;
                if (request.DateTime != DateTime.MinValue)
                {
                    a = await _contestRespository.GetWorkinContestsPaginated(request.DateTime.Value);
                }
                else
                {
                    a = a = await _contestRespository.GetWorkinContestsPaginated(DateTime.UtcNow);
                }
                return new Response<List<Contest>?> { succeeded = true, data = a };
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

                return new Response<List<Contest>?> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
