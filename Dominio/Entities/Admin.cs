using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [FirestoreData]
    public class Admin
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? TokenLogin { get; set; }
        [FirestoreProperty]
        public string? Email{ get; set; }
        [FirestoreProperty]
        public string? Password{ get; set; }
        [FirestoreProperty]
        public string? Name { get; set; }
        [FirestoreProperty]
        public string? LastName { get; set; }
    }
}
