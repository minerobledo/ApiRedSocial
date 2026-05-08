using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities;
using Domain.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Notificatoins.Command.ResponceRequestFacePhoto
{
    public class ResponceRequestFacePhotoCommandHandler : IRequestHandler<ResponseRequestFacePhotoCommand, Response<bool?>>
    {
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IPostRepository _postRepository;
        private readonly IAuthService _authService;
        public ResponceRequestFacePhotoCommandHandler(IPostRepository postRepository, IFirebaseMessagingRepository firebaseMessagingRepository, IProfileRepository profileRepository, IJwtTokenService jwtTokenService, IAuthService authService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _firebaseMessagingRepository = firebaseMessagingRepository;
            _profileRepository = profileRepository;
            _postRepository = postRepository;
        }

        public async Task<Response<bool?>> Handle(ResponseRequestFacePhotoCommand request, CancellationToken cancellationToken)
        {
            if (_authService.HasNullPropertiesLinq(request)) return new Response<bool?> { succeeded = true, data = false };
            try
            {

                if(request.response == false)
                {
                   var a =await _firebaseMessagingRepository.DeleteNotification(request.NotificationId);
                    if(a==true)
                    {
                        return new Response<bool?> { succeeded = true, data= true  };
                    }else
                    {
                        return new Response<bool?> { succeeded = false, message="eror en la vase de datos intente mas tarde" };
                    }
                }else
                {
                    var reseptorProfileId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);

                    var reseptorProfileTask = _profileRepository.GetProfileByIdAsync(reseptorProfileId);
                    var senderProfileTaks = _profileRepository.GetProfileByIdAsync(request.idSender);

                    Task.WaitAll(reseptorProfileTask, senderProfileTaks);

                    var reseptorProfile =  reseptorProfileTask.Result;
                    var senderProfile = senderProfileTaks.Result;

                    var notification = new NotificationEntity()
                    {
                        Title = "Alguien te permitio ver sus fotos privadas!",
                        Body = reseptorProfile!.NameProfile + "  a aceptado tu solisitud, as clic aqui para ver las fotos.",
                        ProfileId = senderProfile!.Id,
                        Type = "RequestFacePhotoAccepted",
                        Data= new Dictionary<string, object?>
                        {
                            {"facePostIds",await _postRepository.GedtALFacePostFromProfileId(reseptorProfileId) }
                        }
                    };

                    List<string> strings = new List<string>();
                    foreach (var item in senderProfile!.User1DeviceTokens!)
                    {
                        strings.Add(item.Token);

                    }
                    foreach (var item in senderProfile.User2DeviceTokens)
                    {
                        strings.Add(item.Token);

                    }
                    var a = await _firebaseMessagingRepository.SendAndSaveNotification(notification, strings);
                    if (!string.IsNullOrWhiteSpace(a)) return new Response<bool?> { data = true, succeeded = true };
                    return new Response<bool?> { data = false, succeeded = true };
                }
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

                return new Response<bool?> { succeeded = false, errors = new List<Exception>() { ex } };
            }




        }
    }
}
