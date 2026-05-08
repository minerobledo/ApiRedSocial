using Aplication.Interfaces.Repository;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Coomand.Penalize
{
    internal class PenalizeProfileCommandHandler : IRequestHandler<PenalizeProfileCommand, Response<bool?>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IEmailService _emailService;

        public PenalizeProfileCommandHandler(IProfileRepository profileRepository, IEmailService emailService)
        {
            _profileRepository = profileRepository;
            _emailService = emailService;
        }

        public async Task<Response<bool?>> Handle(PenalizeProfileCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine("chingatumadre");
            try
            {
                var profile = await _profileRepository.GetProfileByIdAsync(request.ProfileId!);
                if (request.Function == "Ban")
                {
                    var a = await _profileRepository.BanProfile(request.ProfileId!, request.UnBanDate, request.BanReason!);
                    if (!string.IsNullOrEmpty(profile!.User1Email!))
                    {
                        await _emailService.SendEmailWithTemplateAsync(profile!.User1Email!, "TuCuenta ha sido Blokeada", "block_user_template.html", new { BlockReason = request.BanReason });
                    }
                    if (!string.IsNullOrEmpty(profile!.User2Email!))
                    {
                        await _emailService.SendEmailWithTemplateAsync(profile.User2Email!, "TuCuenta ha sido Blokeada", "block_user_template.html", new { BlockReason = request.BanReason });
                    }
                    return new Response<bool?> { succeeded = true, data = a };
                }
                if(request.Function == "RemoveDays")
                {
                    var a = await _profileRepository.RemubeDays(request.ProfileId!, profile!.DateVencetPayment!.Value.AddDays(- request.DeysToRemuve));
                    return new Response<bool?> { succeeded = true, data = a };
                }
                return new Response<bool?> { succeeded = false, data = null };
            }catch (Exception ex)
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
                return new Response<bool?> { succeeded = false,errors = new List<Exception> { ex } };
            }
            
        }
    }
}
