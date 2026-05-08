using Aplication.DTO.InputDto.Friendship;
using Aplication.DTO.OutputDto.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Features.Profiles.Queries.GetProfilesThatVerify
{
    public class GetProfilesThatVerifyQuery: IRequest<Response<List<ProfileShortDto?>>>
    {
       
        
        public string Id { get; set; }
    }
}
