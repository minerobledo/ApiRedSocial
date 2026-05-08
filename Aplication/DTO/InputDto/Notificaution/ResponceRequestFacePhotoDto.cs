using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Notificaution
{
    public class ResponceRequestFacePhotoDto
    {
        public bool responce { get; set; }
        public string? idSender { get; set; }
        public string? NotificationId { get; set; }
    }
}
