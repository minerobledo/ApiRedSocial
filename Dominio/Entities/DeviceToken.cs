using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    [FirestoreData]
    public class DeviceToken
    {
        [FirestoreDocumentId]
        public string? DeviceId { get; set; }                  // ID del dispositivo (puede ser un GUID generado en el frontend)
        [FirestoreProperty]
        public string? Token { get; set; }                     // Device Token de FCM
        [FirestoreProperty]
        public DateTime? LastUpdated { get; set; } = DateTime.UtcNow;            // Última vez que se actualizó el token
    }
}
