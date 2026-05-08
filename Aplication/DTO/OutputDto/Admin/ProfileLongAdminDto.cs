using Domain.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.OutputDto.Profile
{
    public class ProfileLongAdminDto
    {
        public string? Id { get; set; }
        public string? NameProfile { get; set; }
        public string? GodfatherProfileName { get; set; }
        public DateTime? AnniversaryDate { get; set; }
        public int? NumberPersonAuthenticate { get; set; }
        public bool? GetOut { get; set; }
        public string? Interest { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? CoverPhoto { get; set; }
        public string? Description { get; set; }
        public int SponsoredNumbers { get; set; }
        public DateTime? EntryDate { get; set; }
        public bool Ban { get; set; } = false;
        public DateTime UnBanDate { get; set; }
        public string? BanReason { get; set; }
        public DateTime? DateLastPayment { get; set; }
        public bool? AnySuscriber { get; set; }
        public DateTime? DateVencetPayment { get; set; }
        public string? RequestFacePhoto { get; set; }

        //User1
        public string? User1Url_FacePhoto { get; set; }
        public string? User1Name { get; set; }
        public string? User1LastName { get; set; }
        public string? User1Nickname { get; set; }
        public string? User1Email { get; set; }
        public DateTime? User1Birthdate { get; set; }
        public string? User1Gender { get; set; }
        public string? User1Orientation { get; set; }
        public string? User1Traits { get; set; }
        public string? User1Province { get; set; }
        public string? User1Height { get; set; }
        public string? User1Weight { get; set; }
        public string? User1ZodiacSign { get; set; }
        public string? User1EyeColor { get; set; }
        public string? User1HairType { get; set; }
        public string? User1Shaved { get; set; }
        public string? User1EducationLevel { get; set; }

        //User2
        public string? User2Url_FacePhoto { get; set; }
        public string? User2Name { get; set; }
        public string? User2LastName { get; set; }
        public string? User2Nickname { get; set; }
        public string? User2Email { get; set; }
        public DateTime? User2Birthdate { get; set; }
        public string? User2Gender { get; set; }
        public string? User2Orientation { get; set; }
        public string? User2Traits { get; set; }
        public string? User2Province { get; set; }
        public string? User2Height { get; set; }
        public string? User2Weight { get; set; }
        public string? User2ZodiacSign { get; set; }
        public string? User2EyeColor { get; set; }
        public string? User2HairType { get; set; }
        public string? User2Shaved { get; set; }
        public string? User2EducationLevel { get; set; }
    }
}
