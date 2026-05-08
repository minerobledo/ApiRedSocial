using Aplication.Interfaces.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.test.Command
{
    internal class GetJobdsProgramedQuerryCommand: IRequestHandler<GetJobdsProgramedQuerry, Response<List<ITrigger>>>
    {
        private readonly IQuartzJobService _quartzJobService;

        public GetJobdsProgramedQuerryCommand(IQuartzJobService quartzJobService)
        {
            _quartzJobService = quartzJobService;
        }

        public async Task<Response<List<ITrigger>>> Handle(GetJobdsProgramedQuerry request, CancellationToken cancellationToken)
        {
            try
            {
                var responce = await _quartzJobService.GetTaskProgramed();
                return new Response<List<ITrigger>> { data = responce, succeeded = true };
            }
            catch (Exception ex)
            {
                return new Response<List<ITrigger>> { succeeded = false, errors = new List<Exception> { ex } };
            }
           
        }
    }
}
