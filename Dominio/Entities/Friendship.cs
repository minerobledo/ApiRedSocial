using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [FirestoreData]
    public class Friendship
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? Friend1Id { get; set; }
        [FirestoreProperty]
        public string? Friend2Id { get; set; }
        [FirestoreProperty]
        public string? Friend1ProfileName { get; set; }
        [FirestoreProperty]
        public string? Friend2ProfileName { get; set; }
     
        [FirestoreProperty]
        public string? Status { get; set; }
        [FirestoreProperty]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    }
}
