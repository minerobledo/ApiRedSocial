using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Aplication.ResponPattern;
using System.Security.Claims;
using Aplication.DTO.OutputDto.Profile;
namespace Aplication.Features.Frinship.Querys
{
    public class GetFrienchipsQuery : IRequest<Response<List<ProfileShortDto>>>
    {
        
        public string ProfileId { get; set; }
    }
}
