using Domain.Entities.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Repository
{
    public interface IEventRepocitory
    {
        Task<bool?> CreateEvent(EventEntity  eventEntity);
        Task<bool?> UpdateEvent(Dictionary<string,object> eventEntity,string id);
        Task<bool?> DeleteEvent(string id);
        Task<bool?> AceptEvent(string id, DateTime dateTime);
        Task<bool?> CloseEvent(string id);
        Task<List<EventEntity>?> GetEventsAceptedPaginated(DateTime? date);
        Task<EventEntity?> GetEventEntity(string Id);
        Task<List<EventEntity>?> GetEventsPendingPaginated(DateTime? date);
        Task<bool?> AddGuaestToEvent(ProfileEvent guestId, string eventId);
        Task<bool?> RemuveGuaestToEvent(ProfileEvent guestId, string eventId);
    }
}
