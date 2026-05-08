using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Login.reponceProfileAndUser
{
    public class DeviceTokenDto
    {
        public string? DeviceId { get; set; }                  // ID del dispositivo (puede ser un GUID generado en el frontend)
        public string? Token { get; set; }                     // Device Token de FCM

    }
}
