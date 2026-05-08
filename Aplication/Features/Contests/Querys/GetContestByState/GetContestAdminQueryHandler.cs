using Aplication.Interfaces.Repository;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Contests.Querys.GetContestByState
{
    internal class GetContestAdminQueryHandler: IRequestHandler<GetContestAdminQuery, Response<List<Contest>>>
    {
        private readonly IContestRespository _contestRespository;

        public GetContestAdminQueryHandler(IContestRespository contestRespository)
        {
            _contestRespository = contestRespository;
        }

        public async Task<Response<List<Contest>>> Handle(GetContestAdminQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var a = await _contestRespository.GetContestsAdminPaginated( request.StartAfter);
                return new Response<List<Contest>> { succeeded = true, data = a };

            }
            catch(Exception ex)
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

                return new Response<List<Contest>> {succeeded = false,errors = new List<Exception> {ex} };
            }
        }
    }
}
