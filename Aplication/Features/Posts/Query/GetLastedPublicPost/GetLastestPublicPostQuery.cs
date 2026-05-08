using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Posts.Query.GetLastedPublicPost
{
    public class GetLastestPublicPostQuery : IRequest<Response<List<Post>?>>
    {
        public DateTime? date { get; set; }
    }
}
