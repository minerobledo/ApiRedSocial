using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Event
{
    [FirestoreData]
    public class EventEntity
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? EventName { get; set; }
        [FirestoreProperty]
        public string? Description { get; set; }
        [FirestoreProperty]
        public string? Slogan { get; set; }
        [FirestoreProperty]
        public string? Baner { get; set; }
        [FirestoreProperty]
        public int? GuestLimit { get; set; }
        [FirestoreProperty]
        public DateTime EventDate { get; set; }
        [FirestoreProperty]
        public string? OrganizerName { get; set; }
        [FirestoreProperty]
        public string? OrganizerPhone { get; set; }
        [FirestoreProperty]
        public string? OrganizerEmail { get; set; }
        [FirestoreProperty]
        public DateTime CreateAt { get; set; }
        [FirestoreProperty]
        public string? OrganizationName { get; set; }

        [FirestoreProperty]
        public string? Location { get; set; }

        [FirestoreProperty]
        public string? State { get; set; }

        [FirestoreProperty]
        public int? GuestCount { get; set; }
        [FirestoreProperty]
        public List<ProfileEvent>? GuestList { get; set; } = new List<ProfileEvent>();

    }
}
