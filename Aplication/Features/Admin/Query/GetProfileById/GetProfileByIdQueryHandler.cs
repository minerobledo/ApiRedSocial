using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Query.GetProfileById
{
    internal class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, Response<ProfileForAdmin?>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public GetProfileByIdQueryHandler(IProfileRepository profileRepository, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        public async Task<Response<ProfileForAdmin?>> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _profileRepository.GetProfileByIdAsync(request.ProfileId);
                return new Response<ProfileForAdmin?> { succeeded = true, data = _mapper.Map<ProfileForAdmin>(result) };
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
                return new Response<ProfileForAdmin?> { succeeded = false ,errors = new List<Exception> { ex } };
            }
        }
    }
}
