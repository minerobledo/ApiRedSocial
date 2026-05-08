using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Reports.Command.ChangeStateReport
{
    internal class ChangeStateReportCommandHandler: IRequestHandler<ChangeStateReportCommand, Response<bool?>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public ChangeStateReportCommandHandler(IReportRepository reportRepository, IJwtTokenService jwtTokenService)
        {
            _reportRepository = reportRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response<bool?>> Handle(ChangeStateReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var Id = _jwtTokenService.GetAdminIdFromJwt(request.Principal);
                bool? result = null;
                if (request.State == "review")
                {
                    result = await _reportRepository.ChangeState(request.Id,request.State);
                }
                if (request.State == "closed")
                {
                    result = await _reportRepository.ChangeState(request.Id, request.State, request.Result,Id);
                }
                return new Response<bool?> { data = result,succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Response<bool?> { data = null, succeeded = true ,errors = new List<Exception> { ex } };
            }
        }
    }
}
