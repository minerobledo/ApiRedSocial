using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl.Matchers;

namespace Aplication.Interfaces.Services
{
    public interface IQuartzJobService
    {
        Task AddTask<TJob>(string jobId, DateTimeOffset fechaEjecucion, IDictionary<string, object>? jobData = null) where TJob : IJob;
        Task RemoveTask(string jobId);
        Task EditTask<TJob>(string jobId, DateTimeOffset nuevaFechaEjecucion, IDictionary<string, object>? nuevosDatos = null) where TJob : IJob;
        Task<List<ITrigger>> GetTaskProgramed();
       

    }
}
