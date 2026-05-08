using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Aplication.DTO.OutputDto.Profile
{
    [FirestoreData]
    public class ProfileShortDto
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? NameProfile { get; set; }
        [FirestoreProperty]
        public string? ProfilePhoto { get; set; }
        [FirestoreProperty]
        public string? User1Province { get; set; }
        [FirestoreProperty]
        public string? User2Province { get; set; }


    }
}
