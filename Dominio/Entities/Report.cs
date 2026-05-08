using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [FirestoreData]
    public class Report
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? AdminId { get; set; }

        [FirestoreProperty]
        public string? Type { get; set; }

        [FirestoreProperty]
        public string? ReporterProfileId { get; set; }
        [FirestoreProperty]
        public string? ReporterProfileName { get; set; }
        [FirestoreProperty]
        public string? ReportedProfileId { get; set; }
        [FirestoreProperty]
        public string? ReportedProfileName { get; set; }
        [FirestoreProperty]
        public string? State { get; set; }
        [FirestoreProperty]
        public string? Result { get; set; }
        [FirestoreProperty]
        public DateTime? CreateAt { get; set; }
        [FirestoreProperty]
        public DateTime? ClosedAt { get; set; }
    }
}
