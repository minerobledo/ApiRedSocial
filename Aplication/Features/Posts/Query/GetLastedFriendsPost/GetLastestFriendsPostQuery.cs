using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Posts.Query.GetLastedFriendsPost
{
    public class GetLastestFriendsPostQuery: IRequest<Response<List<Post>?>>
    {
        

        public ClaimsPrincipal Principal { get; set; }

        public DateTime? date { get; set; }
    }
}
