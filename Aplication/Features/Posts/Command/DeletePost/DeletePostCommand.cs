
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Posts.Command.DeletePost
{
    public class DeletePostCommand : IRequest<Response<bool?>>
    {
        public ClaimsPrincipal? Principal {  get; set; }
        public string? PostId { get; set; }
    }
}
