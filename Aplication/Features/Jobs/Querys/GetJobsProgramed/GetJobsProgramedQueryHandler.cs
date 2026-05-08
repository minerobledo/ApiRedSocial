using Aplication.Interfaces.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Jobs.Querys.GetJobsProgramed
{
    internal class GetJobsProgramedQueryHandler : IRequestHandler<GetJobsProgramedQuery, Response<List<ITrigger>>>
    {
        private readonly IQuartzJobService _jobService;

        public GetJobsProgramedQueryHandler(IQuartzJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<Response<List<ITrigger>>> Handle(GetJobsProgramedQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var responce = await _jobService.GetTaskProgramed();
                return new Response<List<ITrigger>> { data = responce, succeeded = true };
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

                return new Response<List<ITrigger>> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
