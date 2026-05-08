using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Event
{
    public class AddOrRemuveGuestDto
    {
        public string? EventId { get; set; }
        public bool? Flag   { get; set;}
        public string? ProfilePhoto { get; set; }

        public string? NameProfile { get; set; }
        public string? User1Province { get; set; }
        public string? User2Province { get; set; }
    }
}
