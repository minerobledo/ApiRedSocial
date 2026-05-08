using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Interfaces.Repository;
using Domain.Entities;

namespace Aplication.Features.Contests.Command.CreateContest
{
    public class CreateContestCommandHandler : IRequestHandler<CreateContestCommand, Response<bool?>>
    {
        private readonly IContestRespository _contestRespository;
        public CreateContestCommandHandler(IContestRespository contestRespository)
        {
            _contestRespository = contestRespository;
        }

        public async Task<Response<bool?>> Handle(CreateContestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var contest = new Contest()
                {
                    PostId = new List<string>(),
                    CreateAt = DateTime.UtcNow,
                    StartDate = request.contestToCreate.StartDate,
                    EndDate = request.contestToCreate.EndDate,
                    Title = request.contestToCreate.Title,
                    Description = request.contestToCreate.Description
                };
                return new Response<bool?> { succeeded = true, data = await _contestRespository.CreateContest(contest) };
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

                return new Response<bool?> { succeeded = false, errors = new List<Exception> { ex } };
            }

        }
    }
}
