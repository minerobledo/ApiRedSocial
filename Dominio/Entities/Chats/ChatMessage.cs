using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chats
{
    [FirestoreData]
    public class ChatMessage
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }  // ID único (opcional si Firestore lo genera)

        [FirestoreProperty]
        public string? SenderId { get; set; }  // ID del perfil que envía

        [FirestoreProperty]
        public string? ReceiverId { get; set; }  // ID del usuario que recibe

        [FirestoreProperty]
        public string? UserNameSender { get; set; }  // ID del usuario que envía

        [FirestoreProperty]
        public string? IV { get; set; }
        [FirestoreProperty]
        public string? Message { get; set; }// Contenido del mensaje

        [FirestoreProperty]
        public bool IsRead { get; set; } // Indica si el mensaje ha sido leído

        [FirestoreProperty]
        public DateTime Timestamp { get; set; }  // Hora de envío

    }
}
