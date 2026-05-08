using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Aplication.ResponPattern;
namespace Aplication.Features.Profiles.Queries.EsistTokenGodfather
{
    public class ExistTokenGodfatherQuery: IRequest<Response<bool>>
    {
        public string? token {  get; set; }
    }
}
