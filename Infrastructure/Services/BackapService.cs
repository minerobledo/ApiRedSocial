using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Infrastructure.Services.Jobs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    internal class BackapService : IBackapService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IContestRespository _contestRespository;
        private readonly IQuartzJobService _quartzJobService;

        public BackapService(ISchedulerFactory schedulerFactory, IContestRespository contestRespository, IQuartzJobService quartzJobService)
        {
            _schedulerFactory = schedulerFactory;
            _contestRespository = contestRespository;
            _quartzJobService = quartzJobService;
        }

        public async Task ReprogramarConcursosAsync()
        {
            var iniciar = await _contestRespository.GetContestToInit();
            var finalizar = await _contestRespository.GetContestsToFinalaiz();

            foreach (var c in iniciar)
            {
                var dic = new Dictionary<string, object>()
                {
                    {"ContestID",c.Id }
                };
                await _quartzJobService.AddTask<StartContestJob>(c.Id + "-contest-job", c.StartDate.Value, dic);

            }

            foreach (var c in finalizar)
            {
                var dic = new Dictionary<string, object>()
                {
                    {"ContestID",c.Id }
                };
                await _quartzJobService.AddTask<ConquestFinaliceJob>(c.Id + "-contest-job", c.EndDate.Value, dic);

            }
        }
    }
}
