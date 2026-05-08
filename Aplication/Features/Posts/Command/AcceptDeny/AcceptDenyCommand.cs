using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Aplication.Features.Posts.Command.AcceptDeny
{
    public class AcceptDenyCommand : IRequest<Response<bool?>>
    {
        public bool AcceptDeny { get; set; }
        
        public Post Post {  get; set; } = new Post();


    }
}
