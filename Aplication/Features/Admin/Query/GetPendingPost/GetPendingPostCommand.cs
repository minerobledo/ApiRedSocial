using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Admin.Query.GetPendingPost
{
    public class GetPendingPostCommand: IRequest<Response<List<Post>>>
    {
        public DateTime? dateTime { get; set; }
    }
}
