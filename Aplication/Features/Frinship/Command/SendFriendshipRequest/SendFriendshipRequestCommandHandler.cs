using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Frinship.Command.SendFriendshipRequest
{
    public class SendFriendshipRequestCommandHandler : IRequestHandler<SendFriendshipRequestCommand, Response<bool>>
    {
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;
        private readonly IFriendshipRepository _friendshipRepository;
       
        private readonly IAuthService _authService;
        private readonly ITransactionService _transactionService;
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;
        public SendFriendshipRequestCommandHandler(IProfileRepository profileRepository, IJwtTokenService jwtTokenService, ITransactionService transactionService, IAuthService authService, IFirebaseMessagingRepository firebaseMessagingRepository,  IFriendshipRepository friendshipRepository)
        {
            _jwtTokenService = jwtTokenService;
            _profileRepository = profileRepository;
            _transactionService = transactionService;
            _authService = authService;
            _firebaseMessagingRepository = firebaseMessagingRepository;
            _friendshipRepository = friendshipRepository;
            
        }

        public async Task<Response<bool>> Handle(SendFriendshipRequestCommand request, CancellationToken cancellationToken)
        {
            //obtenemos el ide del perfil que envia la solisitud
            var profileSenderID = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
            var existeriendship = await _friendshipRepository.ExistFrienship(profileSenderID, request.ProfileIdReseptor);
            if (existeriendship == true) return new Response<bool>() { succeeded = true, message = "Ya existe esta amistad", data = false };
            if (existeriendship == null) return new Response<bool>() { succeeded = false, message = "Problemas con la bace de datos", data = false };
            //obtenemos el perfil del Sender
            var profileSender = _profileRepository.GetProfileByIdAsync(profileSenderID);
            //obtenemos el perfil del Reseptor
            var profileReseptor = _profileRepository.GetByIdAsync(request.ProfileIdReseptor);
            Task.WaitAll(profileReseptor, profileSender);

            //lista para mandar todas las notificaciones
            List<string> DeviceToNotificate = new List<string>();
            foreach (var device in profileReseptor.Result.User1DeviceTokens)
            {
                DeviceToNotificate.Add(device.Token);
            }
            if(profileReseptor.Result.User2DeviceTokens != null)
            {
                foreach (var device in profileReseptor.Result.User2DeviceTokens)
                {
                    DeviceToNotificate.Add(device.Token);
                }
            }
            try
            {
                NotificationEntity notificationEntity = new NotificationEntity();
                bool transaction = await _transactionService.ExecuteTransactionAsync(async transaction =>
                {
                    var friendshipId = _friendshipRepository.AddFrienshipTransaction(transaction, profileSender.Result.Id, profileReseptor.Result.Id, profileSender.Result.NameProfile!, profileReseptor.Result.NameProfile!)!;



                    notificationEntity = new NotificationEntity()
                    {
                        Title = "Solicitud de amistad",
                        Body = profileSender.Result.NameProfile + " quiere ser tu amigo.",
                        Type = "FriendshipRequest",
                        ProfileId = request.ProfileIdReseptor,
                        Data = new Dictionary<string, object?>()
                        {
                            {"FrienshipId",friendshipId},
                            {"ImageURL",profileSender.Result.ProfilePhoto }

                        }


                    };
                });

                if (transaction == true && notificationEntity != null)
                {
                    await _firebaseMessagingRepository.SendAndSaveNotification(notificationEntity!, DeviceToNotificate);


                    return new Response<bool>() { succeeded = true, data = true };
                }
                return new Response<bool>() { succeeded = true, data = false };
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

                return new Response<bool>() { succeeded = false, data = false, errors = new List<Exception>() { ex } };
            }
        }
    }
}
