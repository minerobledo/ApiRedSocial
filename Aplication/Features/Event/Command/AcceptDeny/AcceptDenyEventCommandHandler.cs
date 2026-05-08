using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;



using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Event.Command.AcceptDeny
{
    public class AcceptDenyEventCommandHandler: IRequestHandler<AcceptDenyEventCommand, Response<bool?>>
    {
        private readonly IQuartzJobService _quartzJobService;
        private readonly IEventRepocitory _eventRepocitory;
        private readonly IAuthService _authService;

        public AcceptDenyEventCommandHandler(IQuartzJobService quartzJobService, IAuthService authService, IEventRepocitory eventRepocitory)
        {
            _quartzJobService = quartzJobService;
            _authService = authService;
            _eventRepocitory = eventRepocitory;
        }

        public async Task<Response<bool?>> Handle(AcceptDenyEventCommand request, CancellationToken cancellationToken)
        {
            if (_authService.HasNullPropertiesLinq(request)) return new Response<bool?> { succeeded = true, data = false };
            try
            {
                if (request.Status == true)
                {
                    var result = await _eventRepocitory.AceptEvent(request.Id,request.date);
                   
                    return new Response<bool?> { succeeded = true, data = result };

                }
                if (request.Status == false)
                {
                    var result = await _eventRepocitory.DeleteEvent(request.Id);
                    return new Response<bool?> { succeeded = true, data = result };
                }
                return new Response<bool?> { succeeded = false, data = null };
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

                return new Response<bool?> { message = ex.Message, succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
