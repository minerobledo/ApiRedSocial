using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Command.DeleteProfile
{
    internal class DeleteProfileCommandHandler: IRequestHandler<DeleteProfileCommand, Response<bool?>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public DeleteProfileCommandHandler(IProfileRepository profileRepository, IJwtTokenService jwtTokenService)
        {
            _profileRepository = profileRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<bool?>> Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var selfId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
                if (selfId == request.id)
                {
                    var a = await _profileRepository.DeleteAsync(request.id);
                    return new Response<bool?> { succeeded = true, data = a };
                }
                return new Response<bool?> { succeeded = true, data = false };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response<bool?> { succeeded = false ,errors = new List<Exception> { ex } };

            }
        }
    }
}
