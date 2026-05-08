using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities.Event;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Event.Command.CreateEvent
{
    internal class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Response<bool?>>
    {
        private readonly IFileService _fileService;
        private readonly IEventRepocitory _eventRepocitory;
        private readonly IAuthService _authService;

        public CreateEventCommandHandler(IAuthService authService ,IEventRepocitory eventRepocitory,IFileService fileService)
        {
            _fileService = fileService;
            _authService = authService;
            _eventRepocitory = eventRepocitory;
        }

        public async Task<Response<bool?>> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            if (_authService.HasNullPropertiesLinq(request)) return new Response<bool?> { succeeded = true, data = null };
            try
            {
                string fileName1 = $"Baners/{Guid.NewGuid()}_{Path.GetFileName(request.BanerFile.FileName)}";

                // subir archivo al storage
                var url1 = await _fileService.UploadFileAsync(request.BanerFile.OpenReadStream(),fileName1, request.BanerFile.ContentType);


                var eventEntity = new EventEntity()
                {
                    EventName = request.EventName,
                    Description = request.Description,
                    Slogan = request.Slogan,
                    Baner = url1,
                    GuestLimit = request.GuestLimit,
                    EventDate = request.EventDate,
                    OrganizationName = request.OrganizationName,
                    OrganizerEmail = request.OrganizerEmail,
                    OrganizerPhone = request.OrganizerPhone,
                    OrganizerName = request.OrganizerName,
                    State = "pending",
                    CreateAt = DateTime.UtcNow,
                    Location = request.Location

                };
                var result = await _eventRepocitory.CreateEvent(eventEntity);
                if (result != null)
                {
                    return new Response<bool?> { succeeded = true, data = result };
                }
                return new Response<bool?> { succeeded = false, data = result, message = "error en la base de datos" };
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
