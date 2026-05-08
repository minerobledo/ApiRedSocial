using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chats
{
    [FirestoreData]
    public class AdminChatDocument
    {

        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? Subject { get; set; }
        [FirestoreProperty]
        public string? AdminName {  get; set; }
        [FirestoreProperty]
        public string? AdminId { get; set; }
        [FirestoreProperty]
        public string? ProfileName { get; set; }
        [FirestoreProperty]
        public string? ProfileId { get; set; }

        [FirestoreProperty]
        public string? State { get; set; }

        [FirestoreProperty]
        public DateTime CreateAt { get; set; }

    }
}
