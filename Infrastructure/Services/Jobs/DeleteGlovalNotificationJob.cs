using Aplication.Interfaces.Repository;
using NetTopologySuite.Noding;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Jobs
{
    internal class DeleteGlovalNotificationJob: IJob
    {
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;

        public DeleteGlovalNotificationJob(IFirebaseMessagingRepository firebaseMessagingRepository)
        {
            _firebaseMessagingRepository = firebaseMessagingRepository;
        }

        public async Task Execute(IJobExecutionContext contex)
        {
            try
            {
                var a = await _firebaseMessagingRepository.GetGlovalNotification();
                foreach (var item in a)
                {
                    if(item.CreatedAt.AddDays(7).Date == DateTime.Now.Date)
                    {
                        await _firebaseMessagingRepository.DeleteNotification(item.Id);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
    }
}
