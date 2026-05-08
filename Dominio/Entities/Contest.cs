using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Domain.Entities
{
    [FirestoreData]
    public class Contest
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }


        [FirestoreProperty]
        public List<string>? PostId { get; set; }
        [FirestoreProperty]
        public DateTime? StartDate { get; set; }
        
        [FirestoreProperty]
        public DateTime? EndDate { get; set; }
        [FirestoreProperty]
        public DateTime? CreateAt { get; set; }
        [FirestoreProperty]
        public string? Title { get; set; }
        [FirestoreProperty]
        public string? Description { get; set; }
        [FirestoreProperty]
        public string? State { get; set; } = "pending"; //CLosed Working Pending

        [FirestoreProperty]
        public string? WinerPostId {  get; set; }
    }
}
