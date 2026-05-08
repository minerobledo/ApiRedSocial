using Aplication.DTO.OutputDto.Profile;
using Aplication.DTO.Profile.GetProfileQuery;
using Aplication.DTO.Users;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Aplication.ResponPattern;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Queries.GetProfiles
{
    public class GetProfilesLongQueryHandler : IRequestHandler<GetProfilesLongQuery, Response<ProfileLongDto>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;
        public GetProfilesLongQueryHandler(IFirebaseMessagingRepository firebaseMessagingRepository, IAuthService authService ,IJwtTokenService jwtTokenService , IProfileRepository profileRepository, IFriendshipRepository friendshipRepository,IMapper mapper)
        {
            _firebaseMessagingRepository = firebaseMessagingRepository;
            _jwtTokenService = jwtTokenService;
            _authService = authService;
            _mapper = mapper;
            _profileRepository = profileRepository;
            _friendshipRepository = friendshipRepository;

        }

        public async Task<Response<ProfileLongDto>> Handle(GetProfilesLongQuery request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(request.profileName)) return new Response<ProfileLongDto> {succeeded = true, message =" name invalido o vacio",data = null  };
            try
            {
                var mainProfileId =  _jwtTokenService.GetProfileIdFromJwt(request.ClaimsPrincipal);
                var profile = await _profileRepository.GetProfileByNameAsync(request.profileName);
                if (profile == null)
                {
                    return  new Response<ProfileLongDto>() { succeeded = true, data = null };
                }
                var friendshipTask =  _friendshipRepository.GetFriendshipAsinc(mainProfileId,profile.Id );
                var requestFacePhotoTask =  _firebaseMessagingRepository.GetStatusRequestFacePhotoIfExist(mainProfileId,request.profileName);  
                // Esperar a que ambas tareas terminen sin bloquear el hilo
                await Task.WhenAll( friendshipTask, requestFacePhotoTask);

                // Obtener los resultados de las tareas
                
                Friendship? frienship = await friendshipTask;// esta es la linea 59
                
                string? requestFacePhoto =  await requestFacePhotoTask;
                
                var LonG = _mapper.Map<ProfileLongDto>(profile);

                
                if (frienship != null) 
                {
                    LonG.Friendship = frienship.Status;
                }
                else
                {
                    LonG.Friendship = null;
                }
                if(requestFacePhoto != null && requestFacePhoto != "")
                {
                    LonG.RequestFacePhoto = requestFacePhoto;
                }

                return new Response<ProfileLongDto>() { succeeded = true, data = LonG };

               
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea
                return new Response<ProfileLongDto> { succeeded = false,message= "error", data = null, errors = new List<Exception> { ex } };
            }
           
        }
    }
}
