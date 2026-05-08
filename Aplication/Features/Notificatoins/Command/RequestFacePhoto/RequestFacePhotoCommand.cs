using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Notificatoins.Command.RequestFacePhoto
{
    public class RequestFacePhotoCommand : IRequest<Response<bool?>>
    {
        public string? SelfName { get; set; }

        public ClaimsPrincipal? Principal { get; set; }
        public string? ProfilId { get; set; }
    }
}
