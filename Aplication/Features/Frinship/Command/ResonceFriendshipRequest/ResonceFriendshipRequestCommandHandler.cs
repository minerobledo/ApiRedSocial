using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Aplication.ResponPattern;
using Domain.Entities;
using Domain.Entities.Notification;
using MediatR;

namespace Aplication.Features.Frinship.Command.ResonceFriendshipRequest
{
    public class ResonceFriendshipRequestCommandHandler : IRequestHandler<ResonceFriendshipRequestCommand, Response<bool>>
    {
        
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;
        private readonly IAuthService _authService;
        
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public ResonceFriendshipRequestCommandHandler(IJwtTokenService jwtTokenService, IProfileRepository profileRepository, IAuthService authService,  IFriendshipRepository friendshipRepository,IFirebaseMessagingRepository firebaseMessagingRepository)
        {
            _jwtTokenService = jwtTokenService;
            _profileRepository = profileRepository;
            
            _authService = authService;
            _firebaseMessagingRepository = firebaseMessagingRepository;
            _friendshipRepository = friendshipRepository;
            
        }

        public async Task<Response<bool>> Handle(ResonceFriendshipRequestCommand request, CancellationToken cancellationToken)
        {
            var frineship = await _friendshipRepository.GetFriendshipAsinc(request.FriendshipId!);
            if (frineship == null) return new Response<bool> { data = false, succeeded = false };
            Task<Profile> profileSender;
            var idRespocer = _jwtTokenService.GetProfileIdFromJwt(request.principal);
            if (frineship.Friend1Id == idRespocer)
            {
                profileSender = _profileRepository.GetByIdAsync(frineship.Friend2Id);
            }
            else
            {
                profileSender = _profileRepository.GetByIdAsync(frineship.Friend1Id);
            }
            var profileReseptor =_profileRepository.GetByIdAsync(idRespocer);

            Task.WaitAll(profileReseptor,profileSender);

            List<string> DeviceToNotificate = new List<string>();
            foreach (var device in profileSender.Result.User1DeviceTokens)
            {
                DeviceToNotificate.Add(device.Token);
            }
            foreach (var device in profileSender.Result.User2DeviceTokens)
            {
                DeviceToNotificate.Add(device.Token);
            }



            try
            {
                if (request.Responce == true)
                {
                    var flag =  _friendshipRepository.ChangeStatusFrienship(request.FriendshipId!, true);

                    if (flag.Result == true)
                    {
                        var notification = new NotificationEntity()
                        {
                            Title = "Solicitud aceptada",
                            Body = profileReseptor.Result.NameProfile + " a aceptado tu solisitud de amistas, enviale un mensaje!",
                            Type = "FriendchipResponce",
                            ProfileId = profileSender.Result.Id

                        };
                        await _firebaseMessagingRepository.SendAndSaveNotification(notification!, DeviceToNotificate);
                        return new Response<bool> {succeeded = true,data = true};
                    }
                }else if (request.Responce == false)
                {
                    var flag = _friendshipRepository.ChangeStatusFrienship(request.FriendshipId!, false);
                    return new Response<bool> { succeeded = true, data = true };
                }
            } catch (Exception ex)
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

                return new Response<bool> { data = false, succeeded = false };
            }
            return new Response<bool> { data = false, succeeded=false };
        }
    }
}
