using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Notificatoins.Command.RequestFacePhoto
{
    internal class RequestFacePhotoCommandHandler : IRequestHandler<RequestFacePhotoCommand, Response<bool?>>
    {
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;
        private readonly ITransactionService _transactionService;
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;
        public RequestFacePhotoCommandHandler(IProfileRepository profileRepository, ITransactionService transactionService, IFirebaseMessagingRepository firebaseMessagingRepository, IJwtTokenService jwtTokenService)
        {
            _profileRepository = profileRepository;
            _firebaseMessagingRepository = firebaseMessagingRepository;
            _transactionService = transactionService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<bool?>> Handle(RequestFacePhotoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var idSender = _jwtTokenService.GetProfileIdFromJwt(request.Principal);//Sender
                var profileReseptor = await _profileRepository.GetProfileByIdAsync(request.ProfilId);//Reseptor
                if (profileReseptor == null) return new Response<bool?> { data = null, succeeded = true };//Reseptor
                List<string> strings = new List<string>();
                foreach (var item in profileReseptor!.User1DeviceTokens!)//Reseptor
                {
                    strings.Add(item.Token);

                }
                foreach (var item in profileReseptor.User2DeviceTokens)//Reseptor
                {
                    strings.Add(item.Token);

                }
                NotificationEntity notification = new RequestFaceNotification()
                {
                    Title = "Alguien quiere ver tu rostro!!!",
                    Body = request.SelfName + " quiere ver las fotos de rostro en tu cuenta",
                    ProfileId = request.ProfilId,
                    Type = "RequestFacePhoto",
                    SenderId = idSender,
                    ReceptorId = profileReseptor.Id,
                    Status = "pending"
                };

                var a = await _firebaseMessagingRepository.SendAndSaveNotification(notification, strings);
                if (!string.IsNullOrWhiteSpace(a)) return new Response<bool?> { data = true, succeeded = true };
                return new Response<bool?> { data = false, succeeded = true };
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
