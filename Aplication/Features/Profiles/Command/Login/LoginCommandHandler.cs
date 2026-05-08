using Aplication.DTO.InputDto.Login.reponceProfileAndUser;
using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Aplication.Features.Profiles.Command.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<LoginResponseDto>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefresTokenRepository _refresTokenRepository;
        private readonly IMapper _mapper;
        public LoginCommandHandler(IRefresTokenRepository refresTokenRepository ,IMapper mapper, IProfileRepository profileRepository,IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _mapper = mapper;
            _profileRepository = profileRepository;
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _refresTokenRepository = refresTokenRepository;
        }

        public async Task<Response<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (_authService.HasNullPropertiesLinq(request)) return new Response<LoginResponseDto> { succeeded = false, message = "Credenciales invalidas 1" };
                //obtenemos el repositorio, si no existe devolvemos la respuesta de fallo
                var profile = await _profileRepository.GetProfileByTokenAsync(request!.Token!);
                if (profile == null) return new Response<LoginResponseDto> { succeeded = false, message = "Credenciales invalidas 1" };
                if ((DateTime.UtcNow >= profile.DateVencetPayment) || (profile.DateVencetPayment == null))
                {
                    if (profile.AccessLimit != true)
                    {
                        var a = await _profileRepository.ChangesAcesLimit(profile.Id, true);
                        if (a == false)
                        {
                            return new Response<LoginResponseDto> { succeeded = false, message = "Error en bace de datos" };
                        }
                        profile.AccessLimit = true;
                    }
                }
                else
                {
                    if ((profile.AccessLimit != false )&&(profile.PadrinoHaRespondido == true))
                    {
                        var a = await _profileRepository.ChangesAcesLimit(profile.Id, false);
                        if (a == false)
                        {
                            return new Response<LoginResponseDto> { succeeded = false, message = "Error en bace de datos" };
                        }
                        profile.AccessLimit = false;
                    }
                }
                var GFProfile = await _profileRepository.GetProfileByIdAsync(profile!.IdGodfather!);

                //para el usuario 1 preguntamos 
                var email1 = _authService.AuthenticateEmail(profile.User1Email, request.Email);
                var password1 = _authService.AuthenticatePasswordEncript(profile.User1Password!, request.Password!);
                
                //preguntamos por esas variables y devolvemos una respuesta 
                if (email1 && password1)
                {
                    var jwt = _jwtTokenService.GenerateToken(profile.Id!);
                    var refreshToken = _jwtTokenService.GenerateRefeshToken();
                    string? refresID = await _refresTokenRepository.GetRefresTokenDocumentIdIfExist(profile.User1Email);

                    if (refresID == null)
                    {
                        var add = await _refresTokenRepository.AddDocumentAsync(profile.User1Email, profile.Id!, refreshToken, jwt, 1);
                        if (add == null || add == false)
                        {
                            return new Response<LoginResponseDto> { succeeded = false, message = "Error en bace de datos" };
                        };
                    }
                    else if (!string.IsNullOrWhiteSpace(refresID))
                    {
                        var flag = await _refresTokenRepository.UpdateAsync(refreshToken, jwt, refresID);
                        if (flag == null || flag == false)
                        {
                            return new Response<LoginResponseDto> { succeeded = false, message = "Error en bace de datos" };
                        }

                    }
                    var device = _mapper.Map<DeviceToken>(request.DeviceToken);

                    await Task.WhenAll(_profileRepository.ConectedOnOff(profile, true), _profileRepository.AddOrUpdateDeviceTokenAsync(profile,1, device));
                    return new Response<LoginResponseDto>
                    {
                        succeeded = true,
                        data = new LoginResponseDto
                        {
                            JWT = jwt,
                            RefreshToken = refreshToken,
                            SelfProfile = ConvertToSelfProfile(profile,"1")
                        }
                    };
                }
                if(profile.User2Email != null&& profile.User2Password != null)
                {
                    //para el usuario 2 preguntamos 
                    var email2 = _authService.AuthenticateEmail(profile.User2Email, request.Email);
                    var password2 = _authService.AuthenticatePasswordEncript(profile.User2Password!, request.Password!);
                    //preguntamos por esas variables y devolvemos una respuesta 
                    if (email2 && password2)
                    {
                        var jwt = _jwtTokenService.GenerateToken(profile.Id!);
                        var refreshToken = _jwtTokenService.GenerateRefeshToken();
                        var refresID = await _refresTokenRepository.GetRefresTokenDocumentIdIfExist(profile.User2Email);

                        if (refresID == null)
                        {
                            var add = await _refresTokenRepository.AddDocumentAsync(profile.User2Email, profile.Id!, refreshToken, jwt,2);
                            if (add == null || add == false)
                            {
                                return new Response<LoginResponseDto> { succeeded = false, message = "Error en bace de datos" };
                            };
                        }
                        else if (!string.IsNullOrWhiteSpace(refresID))
                        {
                            var flag = await _refresTokenRepository.UpdateAsync(refreshToken, jwt, refresID);
                            if (flag == null || flag == false)
                            {
                                return new Response<LoginResponseDto> { succeeded = false, message = "Error en bace de datos" };
                            }

                        }
                        var device = _mapper.Map<DeviceToken>(request.DeviceToken);

                        await Task.WhenAll(_profileRepository.ConectedOnOff(profile, true), _profileRepository.AddOrUpdateDeviceTokenAsync(profile, 2, device));
                        return new Response<LoginResponseDto>
                        {
                            succeeded = true,
                            data = new LoginResponseDto
                            {
                                JWT = jwt,
                                RefreshToken = refreshToken,
                                SelfProfile = ConvertToSelfProfile(profile,"2")
                            }
                        };
                    }
                }

                return new Response<LoginResponseDto> { succeeded = false, message = "Credenciales invalidas 3" };
            }catch(Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea
                return new Response<LoginResponseDto> { succeeded = false, message = "Error", errors = new List<Exception> { ex } };
            }
        }
        private SelfProfile ConvertToSelfProfile(Domain.Entities.Profile profile, string user)
        {
            // Crear instancia de SelfProfile con los datos generales del perfil
            SelfProfile selfProfile = new SelfProfile
            {
                Id = profile.Id,
                NameProfile = profile.NameProfile,

                TokenGodfather = profile.TokenGodfather,
                AccessLimit = profile.AccessLimit,
                AnniversaryDate = profile.AnniversaryDate,
                NumberPersonAuthenticate = profile.NumberPersonAuthenticate,
                GetOut = profile.GetOut ?? false,
                Interest = profile.Interest,
                ProfilePhoto = profile.ProfilePhoto,
                CoverPhoto = profile.CoverPhoto,
                Description = profile.Description,
                SponsoredNumbers = profile.SponsoredNumbers ?? 0,
                EntryDate = profile.EntryDate
            };
            // Verificar si el usuario está en User1
            if (user == "1")
            {
                selfProfile.UserNumber = 1;
                selfProfile.Nickname = profile.User1Nickname;
                selfProfile.Email = profile.User1Email;
                selfProfile.birthdate = profile.User1BirthDate;
                selfProfile.Gender = profile.User1Gender;
                selfProfile.Orientation = profile.User1Orientation;
                selfProfile.Traits = profile.User1Traits;
                selfProfile.Province = profile.User1Province;
                selfProfile.Height = profile.User1Height;
                selfProfile.Weight = profile.User1Weight;
                selfProfile.ZodiacSign = profile.User1ZodiacSign;
                selfProfile.EyeColor = profile.User1EyeColor;
                selfProfile.HairType = profile.User1HairType;
                selfProfile.Shaved = profile.User1Shaved;
                selfProfile.EducationLevel = profile.User1EducationLevel;
                selfProfile.GeoPoint = profile.User1GeoPoint;
            }
            // Verificar si el usuario está en User2
            else if (user == "2")
            {
                selfProfile.UserNumber = 2;
                selfProfile.Nickname = profile.User2Nickname;
                selfProfile.Email = profile.User2Email;
                selfProfile.birthdate = profile.User2BirthDate;
                selfProfile.Gender = profile.User2Gender;
                selfProfile.Orientation = profile.User2Orientation;
                selfProfile.Traits = profile.User2Traits;
                selfProfile.Province = profile.User2Province;
                selfProfile.Height = profile.User2Height;
                selfProfile.Weight = profile.User2Weight;
                selfProfile.ZodiacSign = profile.User2ZodiacSign;
                selfProfile.EyeColor = profile.User2EyeColor;
                selfProfile.HairType = profile.User2HairType;
                selfProfile.Shaved = profile.User2Shaved;
                selfProfile.EducationLevel = profile.User2EducationLevel;
                selfProfile.GeoPoint = profile.User2GeoPoint;
            }
            return selfProfile;
        }
    }
}
