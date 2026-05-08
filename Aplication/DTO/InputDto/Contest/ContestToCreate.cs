using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Contest
{
    public class ContestToCreate
    {
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
       
        public string? Title { get; set; }
  
        public string? Description { get; set; }
    }
}
