using Aplication.DTO.OutputDto.Profile;
using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Reports.Command.ReportProfile
{
    internal class ReportProfileCommandHandler : IRequestHandler<ReportProfileCommand, Response<bool?>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IReportRepository _reportRepository;

        public ReportProfileCommandHandler(IJwtTokenService jwtTokenService, IReportRepository reportRepository)
        {
            _jwtTokenService = jwtTokenService;
            _reportRepository = reportRepository;
        }

        public async Task<Response<bool?>> Handle(ReportProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var selfId = _jwtTokenService.GetProfileIdFromJwt(request.Principal);
                var report = new Report()
                {
                    ClosedAt = DateTime.UtcNow,
                    State = "pending",
                    ReporterProfileId = selfId,
                    ReporterProfileName = request.ReporterProfileName,
                    ReportedProfileId = request.ReportedProfileId,
                    ReportedProfileName = request.ReportedProfileName,
                    Type = request.Type
                };
                var result = await _reportRepository.AddReport(report);
                return new Response<bool?> { succeeded = true, data = result };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Response<bool?> { succeeded = false, errors = new List<Exception> { ex } };
            }
        }
    }
}
