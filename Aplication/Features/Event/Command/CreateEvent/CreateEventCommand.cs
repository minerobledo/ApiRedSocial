using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Event.Command.CreateEvent
{
    public class CreateEventCommand: IRequest<Response<bool?>>
    {
        public string? EventName { get; set; }
        public string? Description { get; set; }
        public string? Slogan { get; set; }
        public IFormFile? BanerFile { get; set; }
        public int? GuestLimit { get; set; } = null;
        public DateTime EventDate { get; set; }
        public string? OrganizerName { get; set; }
        public string? OrganizerPhone { get; set; }
        public string? OrganizerEmail { get; set; }
        public string? OrganizationName { get; set; }
        public string? Location { get; set; }

    }
}
