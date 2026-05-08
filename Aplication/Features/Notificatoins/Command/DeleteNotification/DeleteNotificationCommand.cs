using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Notificatoins.Command.DeleteNotification
{
    public class DeleteNotificationCommand: IRequest<Response<bool?>>
    {
        public string? PostId {  get; set; }
        public ClaimsPrincipal? Principal { get; set; } = null;
    }
    public class DeleteNotificationCommandHandler: IRequestHandler<DeleteNotificationCommand, Response<bool?>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;

        public DeleteNotificationCommandHandler(IJwtTokenService jwtTokenService, IFirebaseMessagingRepository firebaseMessagingRepository)
        {
            _jwtTokenService = jwtTokenService;
            _firebaseMessagingRepository = firebaseMessagingRepository;
        }

        public async Task<Response<bool?>> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var id = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
                var a = await _firebaseMessagingRepository.DeleteNotification(request.PostId, id);
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
