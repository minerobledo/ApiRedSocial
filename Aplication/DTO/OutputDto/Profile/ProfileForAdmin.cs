using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Google.Cloud.Firestore;

namespace Aplication.DTO.OutputDto.Profile
{
    public class ProfileForAdmin
    {
        
        //campos obligatorios 
        [FirestoreDocumentId]
        public string? Id { get; set; }

        [FirestoreProperty]
        public string? NameProfile { get; set; } //lo pasa el fontend

        [FirestoreProperty]
        public Dictionary<string, string?> UsersDevice { get; set; } = new Dictionary<string, string?>();//lo registro yo

            

        [FirestoreProperty]
        public string? TokenGodfather { get; set; } //lo genero yo

        [FirestoreProperty]
        public string? TokenLogin { get; set; }//lo genero yo

        [FirestoreProperty]
        public bool AccessLimit { get; set; } = true; // por default en true



        [FirestoreProperty]
        public List<string> Notifications { get; set; } = new List<string>();

        // Parejas
        [FirestoreProperty]
        public DateTime? AnniversaryDate { get; set; }//lo pasa el fontend

        [FirestoreProperty]
        public bool? GetOut { get; set; }//lo pasa el fontend








        [FirestoreProperty]
        public string? Interest { get; set; }

        [FirestoreProperty]
        public string? ProfilePhoto { get; set; }

        [FirestoreProperty]
        public string? CoverPhoto { get; set; }

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        public int? SponsoredNumbers { get; set; } = 0;

        [FirestoreProperty]
        public string? CurrentLocation { get; set; }

        [FirestoreProperty]
        public bool Connected { get; set; } = false;

        [FirestoreProperty]
        public string? IdGodfather { get; set; }

        [FirestoreProperty]
        public DateTime? EntryDate { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public bool Ban { get; set; } = false;

        [FirestoreProperty]
        public DateTime? UnBanDate { get; set; }

        [FirestoreProperty]
        public string? BanReason { get; set; }

        [FirestoreProperty]
        public bool? PadrinoHaRespondido { get; set; } = null;

        [FirestoreProperty]
        public bool? LocationActive { get; set; }


        [FirestoreProperty]
        public List<string>? FriendshipsList { get; set; } = new List<string>();

        [FirestoreProperty]
        public bool IsHome { get; set; }

        [FirestoreProperty]
        public int NumberPersonAuthenticate { get; set; }
        [FirestoreProperty]
        public List<string>? ListProfileAuthenticate { get; set; }
        [FirestoreProperty]
        public DateTime? DateLastPayment { get; set; }
        [FirestoreProperty]
        public bool? AnySuscriber { get; set; }

        [FirestoreProperty]
        public DateTime? DateVencetPayment { get; set; }




        //User1
        [FirestoreProperty]
        public string? User1Nickname { get; set; }
        [FirestoreProperty]
        public string? User1Email { get; set; }
           
        [FirestoreProperty]
        public string? User1PhoneNumber { get; set; }

        [FirestoreProperty]
        public string? User1Name { get; set; }
        [FirestoreProperty]
        public string? User1LastName { get; set; }
        [FirestoreProperty]
        public DateTime? User1BirthDate { get; set; }
        [FirestoreProperty]
        public string? User1Gender { get; set; }
        [FirestoreProperty]
        public string? User1Orientation { get; set; }

        [FirestoreProperty]
        public string? User1Traits { get; set; }
        [FirestoreProperty]
        public string? User1Province { get; set; }
        [FirestoreProperty]
        public string? User1Url_FacePhoto { get; set; }

        [FirestoreProperty]
        public List<DeviceToken>? User1DeviceTokens { get; set; }



        //no obligatorios
        [FirestoreProperty]
        public GeoPoint? User1GeoPoint { get; set; }
        [FirestoreProperty]
        public string? User1GeoHash { get; set; }

        [FirestoreProperty]
        public string? User1Height { get; set; }
        [FirestoreProperty]
        public string? User1Weight { get; set; }
        [FirestoreProperty]
        public string? User1ZodiacSign { get; set; }
        [FirestoreProperty]
        public string? User1EyeColor { get; set; }
        [FirestoreProperty]
        public string? User1HairType { get; set; }
        [FirestoreProperty]
        public string? User1Shaved { get; set; }
        [FirestoreProperty]
        public string? User1EducationLevel { get; set; }

        //User2
        //obligatorios
        [FirestoreProperty]
        public string? User2Nickname { get; set; }

        [FirestoreProperty]
        public string? User2Email { get; set; }
        [FirestoreProperty]
        public string? User2Password { get; set; }
        [FirestoreProperty]
        public string? User2PhoneNumber { get; set; }

        [FirestoreProperty]
        public string? User2Name { get; set; }
        [FirestoreProperty]
        public string? User2LastName { get; set; }
        [FirestoreProperty]
        public DateTime? User2BirthDate { get; set; }

        [FirestoreProperty]
        public string? User2Gender { get; set; }
        [FirestoreProperty]
        public string? User2Orientation { get; set; }

        [FirestoreProperty]
        public string? User2Traits { get; set; }
        [FirestoreProperty]
        public string? User2Province { get; set; }
        [FirestoreProperty]
        public string? User2Url_FacePhoto { get; set; }

        [FirestoreProperty]
        public List<DeviceToken>? User2DeviceTokens { get; set; }



        //no obligatorios
        [FirestoreProperty]
        public GeoPoint? User2GeoPoint { get; set; }
        [FirestoreProperty]
        public GeoPoint? User2GeoHash { get; set; }
        [FirestoreProperty]
        public Dictionary<string, object>? User2Location { get; set; }


        [FirestoreProperty]
        public string? User2Height { get; set; }
        [FirestoreProperty]
        public string? User2Weight { get; set; }
        [FirestoreProperty]
        public string? User2ZodiacSign { get; set; }
        [FirestoreProperty]
        public string? User2EyeColor { get; set; }
        [FirestoreProperty]
        public string? User2HairType { get; set; }
        [FirestoreProperty]
        public string? User2Shaved { get; set; }
        [FirestoreProperty]
        public string? User2EducationLevel { get; set; }
        
    }
}
