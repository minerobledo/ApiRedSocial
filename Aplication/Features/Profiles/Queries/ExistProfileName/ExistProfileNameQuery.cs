using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.ResponPattern;
using MediatR;

namespace Aplication.Features.Profiles.Queries.ExistProfileName
{
    public class ExistProfileNameQuery : IRequest<Response<bool?>>
    {
        public string? ProfileNameToCheck { get; set; }
    }
}
