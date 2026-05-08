using Aplication.DTO.InputDto.Friendship;
using Aplication.Interfaces.Repository;
using Domain.Entities;
using Domain.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Notificatoins.Query.GetGlovalNotifications
{
    public class GetGlovalNotificationsQuerry: IRequest<Response<List<NotificationEntity>?>>
    {
    }
    internal class GetGlovalNotificationsQuerryHandler: IRequestHandler<GetGlovalNotificationsQuerry, Response<List<NotificationEntity>?>>
    {
        private readonly IFirebaseMessagingRepository _repository;

        public GetGlovalNotificationsQuerryHandler(IFirebaseMessagingRepository repository)
        {
            _repository = repository;
        }

        public async Task<Response<List<NotificationEntity>?>> Handle(GetGlovalNotificationsQuerry request, CancellationToken cancellationToken)
        {
            try
            {
                var a = await _repository.GetGlovalNotification();
                return new Response<List<NotificationEntity>?> { data = a,succeeded = true};
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

                return new Response<List<NotificationEntity>?> { succeeded= false,errors = new List<Exception> { ex } };

            }
            
        }
    }
}
