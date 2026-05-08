using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Interfaces.Repository;
using Domain.Entities;
using MediatR;

namespace Aplication.Features.Profiles.Command.CreateProfile
{
    public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand,Unit>
    {
        private readonly IProfileRepository _profileRepository;

        public CreateProfileCommandHandler(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<Unit> Handle(CreateProfileCommand request, CancellationToken cancellationToken)
        {
            var profile = new Profile
            {
                Id = Guid.NewGuid().ToString(),
                NameProfile = request.Name,
               // Users = request.Users,
                //Token = new Random().Next(100000, 999999).ToString() // Generar token único
            };

            await _profileRepository.AddAsync(profile);

            return Unit.Value;  // Asegúrate de retornar "Unit.Value"
        }
    }
}
