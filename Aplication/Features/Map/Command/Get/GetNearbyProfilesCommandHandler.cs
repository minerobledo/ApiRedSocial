using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Map.Command.Get
{
    internal class GetNearbyProfilesCommandHandler : IRequestHandler<GetNearbyProfilesCommand,Response< List<ProfileShortDto>>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public GetNearbyProfilesCommandHandler(IProfileRepository profileRepository, IJwtTokenService jwtTokenService)
        {
            _profileRepository = profileRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<List<ProfileShortDto>>> Handle(GetNearbyProfilesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //foreach (var f in request.Filter)
                //{
                //    Console.WriteLine(f.Key + " :" + f.Value.ToString());
                //}
                foreach (var f in request.Filter)
                {
                    if (string.IsNullOrEmpty(f.Value.ToString()))
                    {
                        request.Filter.Remove(f.Key);
                    }
                }

                //foreach (var f in request.Filter)
                //{
                //    Console.WriteLine(f.Key+ " :"+f.Value.ToString());
                //}
                Console.WriteLine("lat: "+request.lat+", long: "+request.lng);
                var selfId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
                var shortProfile = await _profileRepository.GeProfileInMaps(request.Filter, request.Radius,request.lat, request.lng);
                var responce = new List<ProfileShortDto>();
                foreach (var item in shortProfile)
                {
                    if(item.Id != selfId)
                    {
                        responce.Add(item);
                    }
                }
                return new Response<List<ProfileShortDto>> { succeeded =true, data = responce };
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

                return new Response<List<ProfileShortDto>> { succeeded= false , errors = new List<Exception>() { ex } };
            }

        }
    }
}
