using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Notificatoins.Command.ResponceRequestFacePhoto
{
    public class ResponseRequestFacePhotoCommand : IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? Principal { get; set; }
        public bool response {  get; set; }
        public string? idSender  { get; set; }
        public string? NotificationId {  get; set; }
    }
}
