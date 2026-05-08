using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Entities.Notification
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "NotificationType")]
    [JsonDerivedType(typeof(NotificationEntity), "Base")]
    [JsonDerivedType(typeof(RequestFaceNotification), "FriendRequest")]
    [FirestoreData]
    public class NotificationEntity
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string? Title { get; set; }          // Título de la notificación

        [FirestoreProperty]
        public string? ProfileId { get; set; }      //id del dueño de la notificaion

        [FirestoreProperty]
        public string? Body { get; set; }           // Cuerpo del mensaje

        [FirestoreProperty]
        public string? Type { get; set; }           // Tipo de notificación (info, alerta, etc.)

        [FirestoreProperty]
        public bool IsRead { get; set; } = false;  // Estado de lectura

        [FirestoreProperty]
        public Dictionary<string, object?>? Data { get; set; }
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;   // Fecha de creación
        [FirestoreProperty]
        public virtual string NotificationType { get; set; } = nameof(NotificationEntity);
    }
}
