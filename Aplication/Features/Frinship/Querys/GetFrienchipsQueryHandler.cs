using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Aplication.ResponPattern;
using Aplication.Interfaces.Repository;

using Domain.Entities;
using AutoMapper;
using Aplication.Interfaces.Services;
using System.Security.Cryptography;
using Aplication.DTO.OutputDto.Profile;

namespace Aplication.Features.Frinship.Querys
{
    public class GetFrienchipsQueryHandler : IRequestHandler<GetFrienchipsQuery, Response<List<ProfileShortDto>>>
    {
        private readonly IProfileRepository _profileRepository;
        
        private readonly IFriendshipRepository _friendshipRepository;
        
       
        public GetFrienchipsQueryHandler(IFriendshipRepository friendshipRepository, IProfileRepository profileRepository)
        {
            
            _friendshipRepository = friendshipRepository;
            _profileRepository = profileRepository;
            

        }

        public async Task<Response<List<ProfileShortDto>>?> Handle(GetFrienchipsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ProfileId) )return new Response<List<ProfileShortDto>> { succeeded = true, data= null };
            
            
            try
            {
                var ProfileIDlist = await _friendshipRepository.GetAllFriendsIDpByProfilIdAsinc(request.ProfileId);
                var profiles = await _profileRepository.GetProfileShortListByListId(ProfileIDlist);
             
                
                if (profiles != null && profiles.Count > 0)
                {
                    return new Response<List<ProfileShortDto>> { succeeded=true, data= profiles };
                }
                return new Response<List<ProfileShortDto>> { succeeded = true ,data= new List<ProfileShortDto>() };
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

                return new Response<List<ProfileShortDto>> { succeeded = false, data = null, errors = new List<Exception> { ex } };
            }
        }
    }
}
