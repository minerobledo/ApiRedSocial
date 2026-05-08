using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Register
{
    public class RegisterProfileDto
    {
        
        public string? NameProfile { get; set; }

        public string? TokenGodfather { get; set; }

        public DateTime? AnniversaryDate { get; set; }

        
        public bool? GetOut { get; set; }
       

    }
}
