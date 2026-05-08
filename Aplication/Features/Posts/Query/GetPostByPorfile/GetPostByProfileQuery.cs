using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Aplication.Features.Posts.Query.GetPostByPorfile
{
    public class GetPostByProfileQuery : IRequest<Response<List<Post>?>>
    {
        public ClaimsPrincipal? Principal {  get; set; }
        public string? ProfileId { get; set; }

    }
}
