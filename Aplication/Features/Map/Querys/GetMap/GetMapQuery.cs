using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Map.Querys.GetMap
{
    public class GetMapQuery : IRequest<Response<UsersByProvince?>>
    {
    }
}
