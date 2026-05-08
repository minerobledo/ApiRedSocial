using Aplication.DTO.InputDto.Login.reponceProfileAndUser;
using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Aplication.Features.Profiles.Command.TokenLogin
{
    public class TokenLoginCommandHandler : IRequestHandler<TokenLoginCommand, Response<LoginResponseDto>>
    {

        private readonly IRefresTokenRepository _refresTokenRepository;
        private readonly IProfileRepository _profileRepository;
        
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;
        public TokenLoginCommandHandler(IProfileRepository profileRepository, IMapper mapper, IRefresTokenRepository refresTokenRepository, IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _refresTokenRepository = refresTokenRepository;
            _profileRepository = profileRepository;
            
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
        }

        public async Task<Response<LoginResponseDto>> Handle(TokenLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //preguntamos si esta completo todo
                if (_authService.HasNullPropertiesLinq(request)) return new Response<LoginResponseDto> { succeeded = false, message = "credenciales invalidas 1" }; ;

                //preguntamos si expiste el perfil
                Console.WriteLine("El token es: " + request.Token!);
                var profile = await _profileRepository.GetProfileByTokenAsync(request.Token!);
                if (profile == null) return new Response<LoginResponseDto> { succeeded = false, message = "credenciales invalidas 2" };
                
                if ((DateTime.UtcNow >= profile.DateVencetPayment)||(profile.DateVencetPayment == null))
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
                    if ((profile.AccessLimit != false) && (profile.PadrinoHaRespondido == true))
                    {
                        var a = await _profileRepository.ChangesAcesLimit(profile.Id, false);
                        if (a == false)
                        {
                            return new Response<LoginResponseDto> { succeeded = false, message = "Error en bace de datos" };
                        }
                        profile.AccessLimit = false;
                       
                    }
                }
                //generamos el perfil para devolver
                SelfProfile selfProfile = null;


                var trusted = await _profileRepository.GetTrustedDeviceByDeviceId(request.Device, profile.Id);
               
                if (trusted == 1)
                {
                    //generamos todas las pabadas para devolver
                    var jwt = _jwtTokenService.GenerateToken(profile.Id!);
                    var refreshToken = _jwtTokenService.GenerateRefeshToken();
                    var refresID = await _refresTokenRepository.GetRefresTokenDocumentIdIfExist(profile.Id,1);
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

                    selfProfile = ConvertToSelfProfile(profile, "1");
                    if (profile.IdGodfather == null)
                    {
                        var GFprofil = await _profileRepository.GetProfileByIdAsync(profile.IdGodfather!);
                        selfProfile.GodfatherProfileName = GFprofil.NameProfile;
                    }
                    var device = _mapper.Map<DeviceToken>(request.DeviceToken);
                    await _profileRepository.AddOrUpdateDeviceTokenAsync(profile, 1, device);
                    return new Response<LoginResponseDto>
                    {

                        succeeded = true,
                        message = "credenciales correctas",
                        data = new LoginResponseDto()
                        {
                            JWT = jwt,
                            RefreshToken = refreshToken,
                            SelfProfile = selfProfile
                        }
                    };
                }
                if (trusted == 2)
                {
                    //generamos todas las pabadas para devolver
                    var jwt = _jwtTokenService.GenerateToken(profile.Id!);
                    var refreshToken = _jwtTokenService.GenerateRefeshToken();
                    var refresID = await _refresTokenRepository.GetRefresTokenDocumentIdIfExist(profile.Id,1);
                    if (refresID == null)
                    {
                        var add = await _refresTokenRepository.AddDocumentAsync(profile.User2Email, profile.Id!, refreshToken, jwt, 2);
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

                    selfProfile = ConvertToSelfProfile(profile, "2");
                    if (profile.IdGodfather == null)
                    {
                        var GFprofil = await _profileRepository.GetProfileByIdAsync(profile.IdGodfather!);
                        selfProfile.GodfatherProfileName = GFprofil.NameProfile;
                    }
                    var device = _mapper.Map<DeviceToken>(request.DeviceToken);
                    await _profileRepository.AddOrUpdateDeviceTokenAsync(profile, 2, device);
                    return new Response<LoginResponseDto>
                    {

                        succeeded = true,
                        message = "credenciales correctas",
                        data = new LoginResponseDto()
                        {
                            JWT = jwt,
                            RefreshToken = refreshToken,
                            SelfProfile = selfProfile
                        }
                    };
                }

                return new Response<LoginResponseDto> { succeeded = true, message = "credenciales invalidas 1", data = null };
                


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea
                return new Response<LoginResponseDto>()
                {
                    succeeded = false,
                    message = ex.ToString(),
                    data = null,
                    errors = new List<Exception> { ex }
                };
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
                selfProfile.UserNumber = 1;
            }
            // Verificar si el usuario está en User2
            else if (user == "2")
            {
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
                selfProfile.UserNumber = 2;
            }
            return selfProfile;
        }
    }
}