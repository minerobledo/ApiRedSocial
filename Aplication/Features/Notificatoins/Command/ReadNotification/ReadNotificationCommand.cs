using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Notificatoins.Command.ReadNotification
{
    public class ReadNotificationCommand: IRequest<Response<bool?>>
    {
        public string ? PostId {  get; set; }
    }
    internal class ReadNotificationCommandHandler:IRequestHandler<ReadNotificationCommand, Response<bool?>>
    {
        private readonly IFirebaseMessagingRepository _repository;

        public ReadNotificationCommandHandler(IFirebaseMessagingRepository repository)
        {
            _repository = repository;
        }

        public async Task<Response<bool?>> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var a = await _repository.ReadNotification(request.PostId);
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
