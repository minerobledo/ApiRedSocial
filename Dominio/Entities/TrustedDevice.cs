using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [FirestoreData]
    public class TrustedDevice
    {
        [FirestoreDocumentId]
        public string? DocumentId { get; set; }
        [FirestoreProperty]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? Marca { get; set; }
        [FirestoreProperty]
        public string? Modelo { get; set; }
        [FirestoreProperty]
        public DateTime CreateAt {  get; set; }
        [FirestoreProperty]
        public DateTime LastLoginAt { get; set; }

    }
}
