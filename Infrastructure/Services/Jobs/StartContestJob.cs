using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Jobs
{
    internal class StartContestJob : IJob
    {
        private readonly IContestRespository _contestRespository;
        private readonly IQuartzJobService _quartzJobService;

        public StartContestJob(IContestRespository contestRespository, IQuartzJobService quartzJobService)
        {
            _contestRespository = contestRespository;
            _quartzJobService = quartzJobService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("inicia concurzo");
            string? ContestID = context.JobDetail.JobDataMap.GetString("ContestID");
            var dic = new Dictionary<string, object>()
                {
                    { "ContestID ", ContestID}
                };
            var conquest = await _contestRespository.GetContestById(ContestID);
            var a = await _contestRespository.StartContest(ContestID);
            await _quartzJobService.AddTask<ConquestFinaliceJob>(ContestID + "-finished-contest-job", conquest.EndDate.Value,dic);
                
        }
    }
}
