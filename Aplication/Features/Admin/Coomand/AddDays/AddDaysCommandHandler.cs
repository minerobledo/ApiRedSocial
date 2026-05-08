using Aplication.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Coomand.AddDays
{
    internal class AddDaysCommandHandler: IRequestHandler<AddDaysCommand,Response<bool?>>
    {
        private readonly IProfileRepository _profileRepository;

        public AddDaysCommandHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<Response<bool?>> Handle(AddDaysCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var a = await _profileRepository.AddDeys(request.ProfileID, request.Days);
                return new Response<bool?> { succeeded = true, data = a };
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
