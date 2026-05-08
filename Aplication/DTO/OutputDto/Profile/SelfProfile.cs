using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.OutputDto.Profile
{
    public class SelfProfile
    {
        //profile
        public string? Id { get; set; }
        public string? NameProfile { get; set; }

        public string? GodfatherProfileName { get; set; }
        public string? TokenGodfather { get; set; }
        public bool AccessLimit { get; set; }
        public DateTime? AnniversaryDate { get; set; }
        public int NumberPersonAuthenticate { get; set; }
        public bool GetOut { get; set; }
        public string? Interest { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? CoverPhoto { get; set; }
        public string? Description { get; set; }
        public int SponsoredNumbers { get; set; }
        public DateTime? EntryDate { get; set; }

        //user
        public int UserNumber { get; set; }
        public string? Nickname { get; set; }
        public string? Email { get; set; }
        public DateTime? birthdate { get; set; }
        public string? Gender { get; set; }
        public string? Orientation { get; set; }
        public string? Traits { get; set; }
        public string? Province { get; set; }
        public string? Height { get; set; }
        public string? Weight { get; set; }
        public string? ZodiacSign { get; set; }
        public string? EyeColor { get; set; }
        public string? HairType { get; set; }
        public string? Shaved { get; set; }
        public string? EducationLevel { get; set; }
        public GeoPoint? GeoPoint { get; set; }
    }
}
