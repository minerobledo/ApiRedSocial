using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Event.Command.AddOrRemubeGuest
{
    public class AddOrRemubeGuestCommand : IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? Principal { get; set; } = null;
        public string? EventID { get; set; }
        public bool? funcion { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? NameProfile { get; set; }
        public string? User1Province { get; set; }
        public string? User2Province { get; set; }
    }
}
