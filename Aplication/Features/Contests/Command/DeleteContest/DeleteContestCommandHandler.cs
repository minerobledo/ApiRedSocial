using Aplication.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Contests.Command.DeleteContest
{
    internal class DeleteContestCommandHandler : IRequestHandler<DeleteContestCommand, Response<bool?>>

    {
        private readonly IContestRespository _contestRespository;
        public DeleteContestCommandHandler(IContestRespository contestRespository)
        {
            _contestRespository = contestRespository;
        }

        public async Task<Response<bool?>> Handle(DeleteContestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var a = await _contestRespository.DeleteContest(request.id);
                return new Response<bool?> { succeeded = true,data = a };
            }catch (Exception ex)
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

                return new Response<bool?>()
                {
                    succeeded = false,
                    errors = new List<Exception> { ex }
                };
            }
        }
    }
}
