using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chats
{
    [FirestoreData]
    public class ChatDocument
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }         

        [FirestoreProperty]
        public string? Profile1 { get; set; }  

        [FirestoreProperty]
        public string? Profile2 { get; set; }

        [FirestoreProperty]
        public string? Profile1Name { get; set; }

        [FirestoreProperty]
        public string? Profile2Name { get; set; }


        [FirestoreProperty]
        public DateTime CreateAt { get; set; }  

    }
}
