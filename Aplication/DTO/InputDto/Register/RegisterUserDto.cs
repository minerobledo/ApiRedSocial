using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Register
{
    public class RegisterUserDto
    {
        [FirestoreProperty]
        public string? Nickname { get; set; }

        [FirestoreProperty]
        public string? Email { get; set; }
        [FirestoreProperty]
        public string? Password { get; set; }
        [FirestoreProperty]
        public string? PhoneNumber { get; set; }

        [FirestoreProperty]
        public string? Name { get; set; }
        [FirestoreProperty]
        public string? LastName { get; set; }
        [FirestoreProperty]
        public DateTime BirthDate { get; set; }
        [FirestoreProperty]
        public string? Gender { get; set; }
        [FirestoreProperty]
        public string? Orientation { get; set; }
        [FirestoreProperty]
        public string? Traits { get; set; }
        [FirestoreProperty]
        public string? Province { get; set; }
      

    }
}
