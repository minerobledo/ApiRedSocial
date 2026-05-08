using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Event.Command.AddOrRemubeGuest
{
    internal class AddOrRemubeGuestCommandHandler: IRequestHandler<AddOrRemubeGuestCommand, Response<bool?>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEventRepocitory _eventRepocitory;

        public AddOrRemubeGuestCommandHandler(IJwtTokenService jwtTokenService, IEventRepocitory eventRepocitory)
        {
            _jwtTokenService = jwtTokenService;
            _eventRepocitory = eventRepocitory;
        }

        public async Task<Response<bool?>> Handle(AddOrRemubeGuestCommand request, CancellationToken cancellationToken)
        {
            var selfProfileId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
            if (selfProfileId == null || request.EventID == null) return new Response<bool?>() { succeeded = true, data = null };
            try
            {
                var profileEvent = new ProfileEvent()
                {
                    Id = selfProfileId,
                    ProfilePhoto = request.ProfilePhoto,
                    NameProfile = request.NameProfile,
                    User1Province = request.User1Province,
                    User2Province = request.User2Province
                };

                if(request.funcion == true)
                {
                    var a = await _eventRepocitory.AddGuaestToEvent(profileEvent, request.EventID);
                    
                    return new Response<bool?>() { succeeded = true, data = a };
                }
                if (request.funcion == false)
                {
                    var a = await _eventRepocitory.RemuveGuaestToEvent(profileEvent, request.EventID);
                    return new Response<bool?>() { succeeded = true, data = a };
                }
                return new Response<bool?>() { succeeded = true, data = null };
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

                return new Response<bool?>() {succeeded = false ,errors = new List<Exception> {ex} };
            }
        }
    }
}
