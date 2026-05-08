using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Event
{
    [FirestoreData]
    public class ProfileEvent
    {
        [FirestoreProperty]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? ProfilePhoto { get; set; }
        [FirestoreProperty]
        public string? NameProfile { get; set; }
        [FirestoreProperty]
        public string? User1Province { get; set; }
        [FirestoreProperty]
        public string? User2Province { get; set; }
    }
}
