
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
using static Google.Rpc.Context.AttributeContext.Types;

namespace Aplication.Features.Profiles.Command.Register
{
    public class RegisterCommandHnadler : IRequestHandler<RegisterCommand, Response<bool>>
    {
        //private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IFirebaseMessagingRepository _firebaseMessagingService;
       private readonly IFileService _fileService;


        public RegisterCommandHnadler(IFileService fileService, IEmailService emailService, IFirebaseMessagingRepository firebaseMessagingService, IProfileRepository profileRepository, IMapper mapper, ITransactionService transactionService, IAuthService authService)
        {
            _emailService = emailService;
            _fileService = fileService;  
            _profileRepository = profileRepository;
            _mapper = mapper;
            _transactionService = transactionService;
            _authService = authService;
            _firebaseMessagingService = firebaseMessagingService;

        }

        public async Task<Response<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try 
            {
                //preguntamos si el toquen de padrino coresponde a algun perfil
                var GodatherProfile = await _profileRepository.GetProfileByTokenGodfatherAsync(request.profileDtos!.TokenGodfather!);
                if (GodatherProfile == null)
                {
                    return new Response<bool> { succeeded = false, message = "Registro invalido 1" };
                }


                //Preguntamos si el nombred e perfil es valido 
                if (!_authService.ValidateUserName(request.profileDtos.NameProfile!)) return new Response<bool> { succeeded = false, message = "Registro invalido 2" };

                //preguntamos si el nombre de perfil existe en la base de datos
                var flagProfileName = await _profileRepository.ExistProfileByNameProfileAsync(request.profileDtos.NameProfile!);
                if(flagProfileName == true || flagProfileName == null) return new Response<bool> { succeeded = false, message = "Registro invalido 3" , data= false};

                if (!_authService.IsAdult(request.registerUserDtos1.BirthDate)) return new Response<bool> { succeeded = false, message = "Registro invalido 4", data = false };

                //Preguntamos si el nick name y la contraceña son validas
                if (!_authService.ValidateUserName(request.registerUserDtos1.Nickname!)) return new Response<bool> { succeeded = false, message = "Registro invalido 5" };
                if (!_authService.ValidatePassword(request.registerUserDtos1.Password!)) return new Response<bool> { succeeded = false, message = "Registro invalido 6" };
                //preguntamos si el email y el telefono del user1 existen en la base de datos
                var flagUser1Email = await  _profileRepository.ExistProfileByEmailAsync(request.registerUserDtos1.Email!);
                var flagUser1Phone = await _profileRepository.ExistProfileByPhoneAsync(request.registerUserDtos1.PhoneNumber!);
                
                if(flagUser1Email == true || flagUser1Email == null )
                    return new Response<bool> { succeeded = false, message = "Registro invalido 7" };
                if(flagUser1Phone == true || flagUser1Phone == null)
                    return new Response<bool> { succeeded = false, message = "Registro invalido 7.1" };


                //de existir el usuario2 preguntamos si su telefono y email exsten en la base de datos
                bool? flagUser2Email ;
                bool? flagUser2Phone ;

                if (request.cantidadUsuraios > 1)
                {
                    if (!_authService.IsAdult(request.registerUserDtos1.BirthDate)) return new Response<bool> { succeeded = false, message = "Registro invalido 8", data = false };
                    //Preguntamos si el nick name y la contraceña son validas
                    if (!_authService.ValidateUserName(request.registerUserDtos1.Nickname!)) return new Response<bool> { succeeded = false, message = "Registro invalido 9" };
                    if (!_authService.ValidatePassword(request.registerUserDtos1.Password!)) return new Response<bool> { succeeded = false, message = "Registro invalido 10" };

                    flagUser2Email = await _profileRepository.ExistProfileByEmailAsync(request.registerUserDtos1.Email!);
                    flagUser2Phone = await _profileRepository.ExistProfileByPhoneAsync(request.registerUserDtos1.PhoneNumber!);
                    if (flagUser2Email == true || flagUser2Email == null || flagUser2Phone == true || flagUser2Phone == null)
                        return new Response<bool> { succeeded = false, message = "Registro invalido 11" };
                    if (flagUser2Phone == true || flagUser2Phone == null)
                        return new Response<bool> { succeeded = false, message = "Registro invalido 11.1" };
                    //Preguntamos si tienen los datos de parejas
                    if (request.profileDtos.AnniversaryDate == null && request.profileDtos.GetOut == null) return new Response<bool> { succeeded = false, message = "Registro invalido 12" };
                    if (_authService.HasNullPropertiesLinq(request.profileDtos))
                    {
                        return new Response<bool> { succeeded = false, message = "Registro invalido 13" };
                    }

                    //si son 2 usuarios preguntamos si compraten Correo, numero o contraseña

                    if (request.registerUserDtos1.Email == request.registerUserDtos2.Email)
                        return new Response<bool> { succeeded = false, message = "Registro invalido 14" };
                    if (request.registerUserDtos1.PhoneNumber == request.registerUserDtos2.PhoneNumber)
                        return new Response<bool> { succeeded = false, message = "Registro invalido 15" };
                    if (request.registerUserDtos1.Password == request.registerUserDtos2.Password)
                        return new Response<bool> { succeeded = false, message = "Registro invalido 16" };
                }


            
            

            
                if (request.cantidadUsuraios == 1)
                {

                    //REVISAR ESTO las 2 condiciones tienen que ser lopor lojicas null
                    if (request.profileDtos.AnniversaryDate is null && request.profileDtos.GetOut != null) return new Response<bool> { succeeded = false, message = "Registro invalido 17" };
                    if (request.registerUserDtos1== null && _authService.HasNullPropertiesLinq(request.registerUserDtos1!))
                    {
                        return new Response<bool> { succeeded = false, message = "Registro invalido 18" };
                    }
                    //Preguntamos si los nomres de usuarios, perfil y contraseñas son validos
                    //var psw = _userRepository.ExistUserByEmailNumberAsync(request.registerUserDtos[0].Email);
                    //var email = _userRepository.ExistUserByPhoneNumberAsync(request.registerUserDtos[0].PhoneNumber!);
                    //Task.WaitAll(psw, email);
                    //if (psw.Result || email.Result) return new Response<bool> { succeeded = false, message = "Registro invalido 18" };
                }

                var transaccionResult = await Transaction(request, GodatherProfile.Id!,GodatherProfile);
                if (transaccionResult)
                {
                
                    return new Response<bool>
                    {
                        succeeded = true,
                        message = "Registro realisado correctamente",
                        data = true

                    };
                }
                else
                {
                    return new Response<bool>
                    {
                        succeeded = true,
                        message = "Error en base de datos, intentar mas tarde",
                        data = false
                    };
                }

            
            }catch(Exception ex) 
            {
                Console.WriteLine(ex);
                return new Response<bool>
                {

                    succeeded = false,
                    message = "",
                    data = false
                    
                };
            }
            


        }


        private async Task<bool> Transaction(RegisterCommand request, string id, Domain.Entities.Profile GodatherProfile)
        {
            var profile = await TransformProfile(request);
            profile.IdGodfather = id;
            var transaccionResult = await _transactionService.ExecuteTransactionAsync(async transaction =>
            {
                
                // 🔹 Agregamos el perfil
                profile.Id = await _profileRepository.AddTransactionAsync(transaction, profile);
            });

            // 📌 Si la transacción en Firestore falló, terminamos aquí
            if (!transaccionResult)
            {
                return false;
            }

            try
            {
                var not = new NotificationEntity
                {
                    Title = "Notificacion de apadrinamiento",
                    Body = profile.NameProfile + " quiere que lo apadrines!",
                    Type = "Godather",
                    Data = new Dictionary<string, object?> { { "Id", profile.Id } }

                };
                await Notification(GodatherProfile,not);


                var linck = "http://redselecta.com/#/confirmed-email?token=" + profile.TokenLogin;
                // 📧 Intentamos enviar el correo
                Console.WriteLine(request.registerUserDtos1.Email+" la verga ");
                    await _emailService.SendEmailWithTemplateAsync(request.registerUserDtos1.Email!, "Bienvenido a nuestra plataforma",
                    "confirm_email_template.html",
                     new { senderNickname = request.registerUserDtos1.Nickname, button_link = linck });
                if(request.cantidadUsuraios ==2)
                {
                    await _emailService.SendEmailWithTemplateAsync(request.registerUserDtos2.Email!, "Bienvenido a nuestra plataforma",
                    "confirm_email_template.html",
                     new { senderNickname = request.registerUserDtos2.Nickname, button_link = linck });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al enviar el correo: {ex.Message}");

                // 🔥 Si el correo falla, eliminamos usuario y perfil de Firestore
                try
                {
                    await _transactionService.ExecuteTransactionAsync(transaction =>
                    {
                        var profileBool = _profileRepository.DeleteTransaccionAsync(transaction!, profile.Id!);
                        return Task.CompletedTask;
                    });
                }
                catch (Exception rollbackEx)
                {
                    Console.WriteLine(rollbackEx.Message );
                    return false;
                }
            }
            return true;
        }

        private async Task Notification(Domain.Entities.Profile profile, NotificationEntity notification)
        {
            
            try
            {
                
                var a = new List<string>();
                if(profile.User1DeviceTokens != null)
                {
                    foreach (var device in profile.User1DeviceTokens)
                    {
                        a.Add(device.Token!);  
                 
                    }
                }
                if (profile.User2DeviceTokens != null)
                {
                    foreach (var device in profile.User1DeviceTokens!)
                    {
                        a.Add(device.Token!);
                       
                    }
                }
                await _firebaseMessagingService.SendAndSaveNotification(notification, a);



            }
            catch (Exception ex) { Console.WriteLine(ex); }


        }

       
        private async Task< Domain.Entities.Profile?> TransformProfile(RegisterCommand command)
        {
            if (command == null || command.profileDtos == null)
                return null;

            var profile = new Domain.Entities.Profile
            {
                Id = Guid.NewGuid().ToString(),
                NameProfile = command.profileDtos.NameProfile,
                TokenGodfather = command.profileDtos.TokenGodfather,
                AnniversaryDate = command.profileDtos.AnniversaryDate,
                GetOut = command.profileDtos.GetOut,
                EntryDate = DateTime.UtcNow,
                AccessLimit = true,
                Connected = false,
                Ban = false,
                PadrinoHaRespondido = false,
                IsHome = false,
                NumberPersonAuthenticate = command.cantidadUsuraios,
                
                User1DeviceTokens = new List<DeviceToken>(),
                User2DeviceTokens = new List<DeviceToken>(),
                FriendshipsList = new List<string>(),
                Notifications = new List<string>(),
                
            };

            if (command.cantidadUsuraios == 1)
            {
                string fileName = $"FacePhoto/{Guid.NewGuid()}_{profile.Id}user1";

                // subir archivo al storage
                var url = await _fileService.UploadFileAsync(command.FacePhotoUser1.OpenReadStream(), fileName, command.FacePhotoUser1.ContentType);


                var user1 = command.registerUserDtos1;
                profile.User1Nickname = user1.Nickname;
                profile.User1Email = user1.Email;
                profile.User1Password = _authService.HashinPassword(user1.Password);
                profile.User1PhoneNumber = user1.PhoneNumber;
                profile.User1Name = user1.Name;
                profile.User1LastName = user1.LastName;
                profile.User1BirthDate = user1.BirthDate;
                profile.User1Gender = user1.Gender;
                profile.User1Orientation = user1.Orientation;
                profile.User1Traits = user1.Traits;
                profile.User1Province = user1.Province;
                profile.User1Url_FacePhoto = url;
            }

            if (command.cantidadUsuraios == 2)
            {
                string fileName1 = $"FacePhoto/{Guid.NewGuid()}_{profile.Id} user1";

                // subir archivo al storage
                var url1 = await _fileService.UploadFileAsync(command.FacePhotoUser1.OpenReadStream(), fileName1, command.FacePhotoUser1.ContentType);

                string fileName2 = $"FacePhoto/{Guid.NewGuid()}_{profile.Id} user2";

                // subir archivo al storage
                var url2 = await _fileService.UploadFileAsync(command.FacePhotoUser2.OpenReadStream(), fileName2, command.FacePhotoUser2.ContentType);

                var user1 = command.registerUserDtos1;
                profile.User1Nickname = user1.Nickname;
                profile.User1Email = user1.Email;
                profile.User1Password = _authService.HashinPassword( user1.Password);
                profile.User1PhoneNumber = user1.PhoneNumber;
                profile.User1Name = user1.Name;
                profile.User1LastName = user1.LastName;
                profile.User1BirthDate = user1.BirthDate;
                profile.User1Gender = user1.Gender;
                profile.User1Orientation = user1.Orientation;
                profile.User1Traits = user1.Traits;
                profile.User1Province = user1.Province;
                profile.User1Url_FacePhoto = url1;

                var user2 = command.registerUserDtos2;
                profile.User2Nickname = user2.Nickname;
                profile.User2Email = user2.Email;
                profile.User2Password = _authService.HashinPassword(user2.Password);
                profile.User2PhoneNumber = user2.PhoneNumber;
                profile.User2Name = user2.Name;
                profile.User2LastName = user2.LastName;
                profile.User2BirthDate = user2.BirthDate;
                profile.User2Gender = user2.Gender;
                profile.User2Orientation = user2.Orientation;
                profile.User2Traits = user2.Traits;
                profile.User2Province = user2.Province;
                profile.User2Url_FacePhoto = url2;
            }

            return profile;
        }

    }
}

