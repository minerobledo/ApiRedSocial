using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.OutputDto.Admin
{
    public class LoginAdminDto
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
       
        public string? JWT { get; set; }
        
        public string? RefreshToken { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
    }
}
