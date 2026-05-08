using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Posts.Query.GetPostList
{
    public class GetPostListQuery : IRequest<Response<List<Domain.Entities.Post>?>>
    {
        public List<string>? PostList {  get; set; }
    }
}
