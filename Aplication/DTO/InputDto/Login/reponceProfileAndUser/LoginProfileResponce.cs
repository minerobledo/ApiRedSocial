using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Login.reponceProfileAndUser
{
    public class LoginProfileResponce
    {
        public string? Id { get; set; }
        public string? NameProfile { get; set; }

        public string? GodfatherProfileName { get; set; }
        public string? TokenGodfather { get; set; }
        public string? AccessLimit { get; set; }
        public DateTime? AnniversaryDate { get; set; }
        public int NumberPersonAuthenticate { get; set; }
        public bool GetOut { get; set; }
        public string? Interest { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? CoverPhoto { get; set; }
        public string? Description { get; set; }
        public int SponsoredNumbers { get; set; }
        public DateTime? EntryDate { get; set; }

    }
}
