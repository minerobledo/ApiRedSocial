using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Notification
{
    [FirestoreData]
    public class RequestFaceNotification : NotificationEntity
    {
        [FirestoreProperty]
        public string? SenderId {  get; set; }
        [FirestoreProperty]
        public string? ReceptorId { get; set; }
        [FirestoreProperty]
        public string? Status { get; set; }
        [FirestoreProperty]
        public override string NotificationType { get; set; } = nameof(RequestFaceNotification);

    }
}
