using Aplication.Interfaces.Repository;
using Domain.Entities.Event;
using Domain.Entities.Notification;
using FirebaseAdmin.Messaging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Jobs
{
    internal class EventNotificationJob : IJob
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IFirebaseMessagingRepository _firebaseMessagingRepository;
        private readonly IEventRepocitory _eventRepocitory;

        public EventNotificationJob(IProfileRepository profileRepository, IFirebaseMessagingRepository firebaseMessagingRepository, IEventRepocitory eventRepocitory)
        {
            _profileRepository = profileRepository;
            _firebaseMessagingRepository = firebaseMessagingRepository;
            _eventRepocitory = eventRepocitory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                string EventId = context.JobDetail.JobDataMap.GetString("eventId");
                EventEntity? eventEntity = await _eventRepocitory.GetEventEntity(EventId);
                if (eventEntity != null)
                {
                    var listNotification = new List<Notification>();

                    foreach (var guest in eventEntity.GuestList)
                    {
                        var lista = new List<string>();
                        var notificatio = new NotificationEntity()
                        {
                            Title = "Queda una hora para " + eventEntity.EventName,
                            Body = "El evento " + eventEntity.EventName + " se realizara entro de una hora, en " + eventEntity.Location,
                            ProfileId = guest.Id,
                            Type = "EventNotification"
                        };
                        var a = await _profileRepository.GetDeviceTokenAsync(guest.Id);
                        if (a != null)
                        {
                            foreach (var item in a)
                            {
                                lista.Add(item.Token);
                            }
                            _firebaseMessagingRepository.SendAndSaveNotification(notificatio, lista);
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
