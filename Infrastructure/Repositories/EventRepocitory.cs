using Aplication.Interfaces.Repository;
using Aplication.Interfaces.Services;
using Domain.Entities.Event;
using Google.Cloud.Firestore;
using Infrastructure.Services.Jobs;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Infrastructure.Repositories
{
    internal class EventRepocitory : GenericRepository<EventEntity>, IEventRepocitory
    {
        private readonly IQuartzJobService _quartzJobService;
        public EventRepocitory(IQuartzJobService quartzJobService, FirestoreDb firestoreDb, string collectionName = "EventEntity") : base(firestoreDb, collectionName)
        {
            _quartzJobService = quartzJobService;
        }

        
        public async Task<bool?> CreateEvent(EventEntity eventEntity)
        {
            if (eventEntity == null) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document();
                await docRef.SetAsync(eventEntity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturado:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Tipo: {ex.GetType().FullName}");
                Console.WriteLine("StackTrace:");
                Console.WriteLine(ex.StackTrace); // Acá vas a ver la línea

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Mensaje: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }
                return false;
            }
            throw new NotImplementedException();
        }

        public async Task<bool?> AceptEvent(string id, DateTime dateTime)
        {
            if (id == null) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                var dic = new Dictionary<string, object>()
                {
                    {"eventId",id }
                    };
                await _quartzJobService.AddTask<EventCloseJob>(id + "-CloseEvent", dateTime.AddMinutes(-10), dic);
                await _quartzJobService.AddTask<EventNotificationJob>(id + "-Notification", dateTime.AddMinutes(-5), dic);
                await docRef.UpdateAsync(new Dictionary<string, object> { { "State", "accepted" } });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            throw new NotImplementedException();
        }
        public async Task<bool?> CloseEvent(string id)
        {
            if (id == null) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.UpdateAsync(new Dictionary<string, object> { { "State", "closed" } });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            throw new NotImplementedException();
        }

        public async Task<bool?> DeleteEvent(string id)
        {
            if (id == null) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            throw new NotImplementedException();
        }

        public async Task<List<EventEntity>?> GetEventsAceptedPaginated(DateTime? date)
        {
            try
            {
                var query = _firestoreDb.Collection(_collectionName).WhereEqualTo("State", "accepted").OrderBy("CreateAt").Limit(10);
                // Ordenamos por NameProfile

                if (date.HasValue)
                {
                    query = query.StartAfter(date); // Continuamos desde el último documento de la página anterior
                }
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                var list = new List<EventEntity>();
                foreach (var entity in snapshot)
                {
                    list.Add(entity.ConvertTo<EventEntity>());
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            throw new NotImplementedException();
        }

        public async Task<List<EventEntity>?> GetEventsPendingPaginated(DateTime? date)
        {
            try
            {
                var query = _firestoreDb.Collection(_collectionName).WhereEqualTo("State", "pending").OrderBy("CreateAt").Limit(10);
                // Ordenamos por NameProfile

                if (date.HasValue)
                {
                    query = query.StartAfter(date); // Continuamos desde el último documento de la página anterior
                }
                QuerySnapshot snapshot = await query.GetSnapshotAsync();
                var list = new List<EventEntity>();
                foreach (var entity in snapshot)
                {
                    list.Add(entity.ConvertTo<EventEntity>());
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            throw new NotImplementedException();
        }

        public async Task<bool?> UpdateEvent(Dictionary<string, object> eventEntity,string id)
        {
            if (eventEntity == null) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(id);
                await docRef.UpdateAsync(eventEntity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            
        }

        public async Task<bool?> AddGuaestToEvent(ProfileEvent guest, string eventId)
        {
            if (guest == null || string.IsNullOrEmpty(eventId)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(eventId);
                var snapshot = await docRef.GetSnapshotAsync();
                var eventEntity = snapshot.ConvertTo<EventEntity>();
                if (eventEntity.GuestList != null )
                {
                    bool flag = true;
                    foreach (var item in eventEntity.GuestList)
                    {
                        if (item.Id == guest.Id)
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        eventEntity.GuestList.Add(guest);
                        eventEntity.GuestCount = eventEntity.GuestCount + 1;
                    }
                }
                if (eventEntity.GuestList == null)
                {
                    eventEntity.GuestList = new List<ProfileEvent> { guest };
                    eventEntity.GuestCount = +1;
                }

                await docRef.SetAsync(eventEntity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<bool?> RemuveGuaestToEvent(ProfileEvent guest, string eventId)
        {
            if (guest == null || string.IsNullOrEmpty(eventId)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(eventId);
                var snapshot = await docRef.GetSnapshotAsync();
                var eventEntity = snapshot.ConvertTo<EventEntity>();
                if (eventEntity.GuestList != null )
                {
                    eventEntity.GuestList?.RemoveAll(g => g.Id == guest.Id);
                    eventEntity.GuestCount = (eventEntity.GuestCount == 0) ? eventEntity.GuestCount = 0 : eventEntity.GuestCount - 1;
                }
                if (eventEntity.GuestList == null)
                {
                    eventEntity.GuestList = new List<ProfileEvent> { guest };
                    eventEntity.GuestCount = (eventEntity.GuestCount == 0) ? eventEntity.GuestCount = 0 : eventEntity.GuestCount - 1;
                }
                await docRef.SetAsync(eventEntity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<EventEntity?> GetEventEntity(string Id)
        {
            if(string.IsNullOrEmpty(Id)) return null;
            try
            {
                var docRef = _firestoreDb.Collection(_collectionName).Document(Id);
                var snapshot = await docRef.GetSnapshotAsync();
                return snapshot.ConvertTo<EventEntity>();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            
        }
    }
}
