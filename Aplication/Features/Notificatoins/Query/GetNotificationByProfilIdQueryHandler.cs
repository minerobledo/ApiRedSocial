using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Aplication.ResponPattern;
using Domain.Entities.Notification;
using MediatR;

namespace Aplication.Features.Notificatoins.Query
{
    public class GetNotificationByProfilIdQueryHandler : IRequestHandler<GetNotificationByProfilIdQuery, Response<List<NotificationEntity>>>
    {
        private readonly IAuthService _authSrevice;
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;
        private readonly IJwtTokenService _jwtTokenService;
        public GetNotificationByProfilIdQueryHandler(IAuthService authService,IJwtTokenService jwtTokenService, IFirebaseMessagingRepository firebaseMessagingRepository)
        {
            _jwtTokenService = jwtTokenService;
            _authSrevice = authService;
            _firebaseMessagingRepository = firebaseMessagingRepository;

        }
        public async Task<Response<List<NotificationEntity>>> Handle(GetNotificationByProfilIdQuery request, CancellationToken cancellationToken)
        {
            var ProfileID = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
            if (ProfileID == null) return new Response<List<NotificationEntity>>() { succeeded = true,message="datos invalidos", data= null };
            try
            {
                return new Response<List<NotificationEntity>>()
                {
                    succeeded = true,
                    data = await _firebaseMessagingRepository.GetNotificationsByProfilId(ProfileID!)
                };

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

                return new Response<List<NotificationEntity>>()
                {
                    succeeded = true,
                    message = ex.Message,
                    errors = new List<Exception>() { ex },
                    data = null
                };
            }

        }
    }
}
