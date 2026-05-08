using Aplication.DTO.OutputDto.Admin;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Coomand.Login
{
    public class AdminLoginCommand: IRequest<Response<LoginAdminDto>>
    {
        public string? TokenLogin { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
