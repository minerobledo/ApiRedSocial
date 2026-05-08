using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;

using Quartz;
using Quartz.Impl.Matchers;
using Infrastructure.Services.Jobs;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class QuartzJobService: IQuartzJobService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        

        public QuartzJobService(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
            
        }

        public async Task<IScheduler> GetSchedulerAsync()
        {
            return await _schedulerFactory.GetScheduler();
        }
        
        public async Task AddTask<TJob>(string jobId, DateTimeOffset fechaEjecucion, IDictionary<string, object>? jobData = null)
    where TJob : IJob
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            // Crear la instancia del Job con el tipo genérico
            var jobBuilder = JobBuilder.Create<TJob>()
                .WithIdentity(jobId, "contest-jobs");

            if (jobData != null)
            {
                var dataMap = new JobDataMap(jobData);
                jobBuilder = jobBuilder.UsingJobData(dataMap);
            }

            var job = jobBuilder.Build();

            // Definir el trigger (cuándo ejecutarlo)
            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobId}-trigger", "contest-triggers")
                .StartAt(fechaEjecucion) // Se ejecuta en la fecha indicada
                .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow()) // Si se retrasa, ejecuta inmediatamente
                .Build();

            // Programar la tarea
            await scheduler.ScheduleJob(job, trigger);
        }
        public async Task RemoveTask(string jobId)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey(jobId, "contest-jobs");
            await scheduler.DeleteJob(jobKey);
        }
        
        public async Task EditTask<TJob>(string jobId, DateTimeOffset nuevaFechaEjecucion, IDictionary<string, object>? nuevosDatos = null)
    where TJob : IJob
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            // Verifica si la tarea existe antes de eliminarla
            var jobKey = new JobKey(jobId, "contest-jobs");
            if (await scheduler.CheckExists(jobKey))
            {
                await scheduler.DeleteJob(jobKey);
            }

            // Volver a agregar la tarea con los nuevos valores
            await AddTask<TJob>(jobId, nuevaFechaEjecucion, nuevosDatos);
        }
        public async Task<List<ITrigger>> GetTaskProgramed()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            var tareas = new List<ITrigger>();

            foreach (var jobKey in jobKeys)
            {
                var triggers = await scheduler.GetTriggersOfJob(jobKey);
                foreach (var trigger in triggers)
                {
                    tareas.Add(trigger);
                }
            }

            return tareas;
        }
    }
}
