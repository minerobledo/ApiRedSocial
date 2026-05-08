using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Coomand.DeleteEverithing
{
    internal class DeleteEverithingCommandHandler: IRequestHandler<DeleteEverithingCommand, Response<bool?>>
    {
        private readonly IAdminRepocitory _adminRepocitory;
        private readonly IContestRespository _contestRespository;
        private readonly IEventRepocitory _eventRepocitory;
        private readonly IPostRepository _postRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IQuartzJobService _quartzJobService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IReportRepository _reportRepository;

        public DeleteEverithingCommandHandler(IReportRepository reportRepository, IAdminRepocitory adminRepocitory, IContestRespository contestRespository, IEventRepocitory eventRepocitory, IPostRepository postRepository, IProfileRepository profileRepository, IQuartzJobService quartzJobService, IJwtTokenService jwtTokenService)
        {
            _adminRepocitory = adminRepocitory;
            _contestRespository = contestRespository;
            _eventRepocitory = eventRepocitory;
            _postRepository = postRepository;
            _profileRepository = profileRepository;
            _quartzJobService = quartzJobService;
            _jwtTokenService = jwtTokenService;
            _reportRepository = reportRepository;
        }

        public async Task<Response<bool?>> Handle(DeleteEverithingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var adminId = _jwtTokenService.GetAdminIdFromJwt(request.principal!);
                var flag = await _adminRepocitory.ExistAdminByID(adminId!);
                if (flag == true)
                {
                    bool? a;
                    switch (request.Type)
                    {
                        case "Contest":
                            a = await _contestRespository.DeleteContest(request.Id!);

                            break;
                        case "Event":
                            a = await _eventRepocitory.DeleteEvent(request.Id!);
                            break;
                        case "Post":
                            a = await _postRepository.DeletePostById(request.Id!);
                            break;
                        case "Profile":
                            a = await _profileRepository.DeleteAsync(request.Id!);
                            break;
                        case "Report":
                            a = await _reportRepository.DeleteReport(request.Id!);
                            break;

                        default:
                            a = null;
                            break;
                    }
                    return new Response<bool?> { succeeded = true, data = a };
                }
                return new Response<bool?> { succeeded = true, data = false };
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
                return new Response<bool?> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
