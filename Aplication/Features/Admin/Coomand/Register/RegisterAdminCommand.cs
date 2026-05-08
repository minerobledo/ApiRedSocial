using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Coomand.Register
{
    public class RegisterAdminCommand: IRequest<Response<bool?>>
    {
        

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Name { get; set; }
        public string? LastnameName { get; set; }
    }
}
