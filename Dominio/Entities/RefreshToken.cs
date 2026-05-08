using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [FirestoreData]
    public class RefreshToken 
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string? JwtToken { get; set; }
        [FirestoreProperty]
        public string? RefreshTokenValue { get; set; }
        [FirestoreProperty]
        public string? UserEmail { get; set; }
        [FirestoreProperty]
        public string? ProfileId { get; set; }
        [FirestoreProperty]
        public int? User { get; set; }
        [FirestoreProperty]
        public DateTime? ExpiresAt { get; set; }

       

    }
}
