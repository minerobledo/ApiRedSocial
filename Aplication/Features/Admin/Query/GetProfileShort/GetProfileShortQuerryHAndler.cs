using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Query.GetProfileShort
{
    internal class GetProfileShortQuerryHAndler: IRequestHandler<GetProfileShortQuerry, Response<List<ProfileShortDto>>>
    {
        private readonly IProfileRepository _profileRepository;

        public GetProfileShortQuerryHAndler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<Response<List<ProfileShortDto>>> Handle(GetProfileShortQuerry request, CancellationToken cancellationToken)
        {

            try
            {
                var filter = new Dictionary<string, object>();
                if (request.Parameters != null)
                {
                    foreach (var item in request.Parameters)
                    {
                        if (item.Key == "Ban") filter.Add(item.Key, item.Value);
                        if (item.Key == "Connected") filter.Add(item.Key, item.Value);
                        if (item.Key == "PadrinoHaRespondido") filter.Add(item.Key, item.Value);
                        if (item.Key == "NumberPersonAuthenticate") filter.Add(item.Key, item.Value);
                        if (item.Key == "NameProfilePrefixes") filter.Add(item.Key, item.Value);
                        if (item.Key == "PaymentPending") filter.Add(item.Key, item.Value);
                        if (item.Key == "GoingExpiredSubscription") filter.Add(item.Key, item.Value); 
                        if (item.Key == "ExpiredSubscription") filter.Add(item.Key, item.Value);
                    }
                }
                var result = new List<ProfileShortDto>();
                if (request.StardtAfter != null)
                {
                    result = await _profileRepository.GetProfileByFilterAsync(filter, request.StardtAfter);
                }
                else
                {
                    result = await _profileRepository.GetProfileByFilterAsync(filter);
                }
                return new Response<List<ProfileShortDto>> { succeeded = true, data = result };

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
                return new Response<List<ProfileShortDto>> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
