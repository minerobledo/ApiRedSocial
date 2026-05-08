using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Domain.Entities
{
    [FirestoreData]
    public class Post
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? IdPublisher {  get; set; }

        [FirestoreProperty]
        public string? ProfileNamePublisher { get; set; }
        [FirestoreProperty]
        public string? Url { get; set; }
        [FirestoreProperty]
        public string? Status { get; set; } // Acepted, Pending, Rejected
        [FirestoreProperty]
        public bool? IsFace { get; set; }
        [FirestoreProperty]
        public string? PostType { get; set; } //Public, Friends
        [FirestoreProperty]
        public DateTime? CreateAt { get; set; }

        [FirestoreProperty]
        public string? ContestId {  get; set; }
        [FirestoreProperty]
        public string? Description  { get; set; }
        [FirestoreProperty]
        public int ? LikesCount { get; set; }

        [FirestoreProperty]
        public List<string>? Likes { get; set; } //Lista de ids de usuarios que dan megusta
    }
}
