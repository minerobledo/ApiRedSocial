using Aplication.DTO.InputDto.Profile;
using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Api.Gax.Grpc.Gcp.AffinityConfig.Types;

namespace Aplication.Features.Profiles.Command.UpdateProfil
{
    internal class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Response<SelfProfile?>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IProfileRepository _profileRepository;
        private readonly IFileService _fileService;

        public UpdateProfileCommandHandler( IJwtTokenService jwtTokenService, IProfileRepository profileRepository, IFileService fileService)
        {
            _jwtTokenService = jwtTokenService;
            _profileRepository = profileRepository;
            _fileService = fileService;
        }

        public async Task<Response<SelfProfile?>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var SelfId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
            try
            {
                Console.WriteLine(SelfId);
                if (SelfId == null) return new Response<SelfProfile?> { succeeded = false, data = null };
                var selfProfile = await _profileRepository.GetProfileByIdAsync(SelfId);
                if (selfProfile == null) return new Response<SelfProfile?> { succeeded = false, data = null };
                var newProfile = await TransformProfile(selfProfile, request.profileEdit);
                var a = await _profileRepository.UpdateProfileById( newProfile);
                Console.WriteLine(a.ToString());
                return new Response<SelfProfile?> { succeeded = true, data = ConvertToSelfProfile(selfProfile,request.profileEdit.User) };
            }
            catch (Exception ex)
            {
                
                Console.WriteLine(ex.ToString);
                return new Response<SelfProfile?> { succeeded = false, errors = new List<Exception> { ex } };
            }
            
        }
        private async Task< Domain.Entities.Profile> TransformProfile(Domain.Entities.Profile profile, ProfileEditDto dto)
        {
            if(dto.CoverPhoto!= null)
            {
                string fileName = $"CoverPhoto/{Guid.NewGuid()}_{Path.GetFileName(dto.CoverPhoto.FileName)}";

                // subir archivo al storage
                var url = await _fileService.UploadFileAsync(dto.CoverPhoto.OpenReadStream(), fileName, dto.CoverPhoto.ContentType);
                profile.CoverPhoto = url;
            }
            if (dto.ProfilePhoto != null)
            {
                string fileName1 = $"ProfilePhoto/{Guid.NewGuid()}_{Path.GetFileName(dto.ProfilePhoto.FileName)}";

                // subir archivo al storage
                var url1 = await _fileService.UploadFileAsync(dto.ProfilePhoto.OpenReadStream(), fileName1, dto.ProfilePhoto.ContentType);
                profile.ProfilePhoto = url1;
            }

            
            profile.GetOut = dto.GetOut ;
            Console.WriteLine( dto.Description );
            if(dto.Description != null) profile.Description = dto.Description;
            if(dto.User == 1)
            {
                if (dto.Nickname != null)
                    profile.User1Nickname = dto.Nickname;
                if (dto.Gender != null)
                    profile.User1Gender= dto.Gender;
                if (dto.Orientation != null)
                    profile.User1Orientation = dto.Orientation;
                if (dto.Traits != null)
                    profile.User1Traits = dto.Traits;
                if (dto.Province != null)
                    profile.User1Province = dto.Province;
                if (dto.Height != null)
                    profile.User1Height = dto.Height;
                if (dto.Weight != null)
                    profile.User1Weight = dto.Weight;
                if (dto.ZodiacSign != null)
                    profile.User1ZodiacSign = dto.ZodiacSign;
                if (dto.EyeColor != null)
                    profile.User1EyeColor = dto.EyeColor;
                if (dto.HairType != null)
                    profile.User1HairType = dto.HairType;
                if (dto.Shaved != null)
                    profile.User1Shaved = dto.Shaved;
                if (dto.EducationLevel != null)
                    profile.User1EducationLevel = dto.EducationLevel;
            }
            if (dto.User == 2)
            {
                if (dto.Nickname != null)
                    profile.User2Nickname = dto.Nickname;
                if (dto.Gender != null)
                    profile.User2Gender = dto.Gender;
                if (dto.Orientation != null)
                    profile.User2Orientation = dto.Orientation;
                if (dto.Traits != null)
                    profile.User2Traits = dto.Traits;
                if (dto.Province != null)
                    profile.User2Province = dto.Province;
                if (dto.Height != null)
                    profile.User2Height = dto.Height;
                if (dto.Weight != null)
                    profile.User2Weight = dto.Weight;
                if (dto.ZodiacSign != null)
                    profile.User2ZodiacSign = dto.ZodiacSign;
                if (dto.EyeColor != null)
                    profile.User2EyeColor = dto.EyeColor;
                if (dto.HairType != null)
                    profile.User2HairType = dto.HairType;
                if (dto.Shaved != null)
                    profile.User2Shaved = dto.Shaved;
                if (dto.EducationLevel != null)
                    profile.User2EducationLevel = dto.EducationLevel;
            }
            return profile;
        }

        private SelfProfile ConvertToSelfProfile(Domain.Entities.Profile profile, int user)
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
            if (user == 1)
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
            else if (user == 2)
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
