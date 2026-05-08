using Aplication.Interfaces.Repository;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Jobs
{
    public class EventCloseJob : IJob
    {
        private readonly IEventRepocitory _eventRepocitory;

        public EventCloseJob(IEventRepocitory eventRepocitory)
        {
            _eventRepocitory = eventRepocitory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            string? EventId = context.JobDetail.JobDataMap.GetString("eventId");
            try
            {
                _eventRepocitory.CloseEvent(EventId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.CompletedTask;
        }
    }
}
