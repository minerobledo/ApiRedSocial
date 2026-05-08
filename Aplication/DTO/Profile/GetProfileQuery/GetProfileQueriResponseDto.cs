using Aplication.DTO.OutputDto.Profile;
using Aplication.DTO.Users;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.Profile.GetProfileQuery
{
    public class GetProfileQueriResponseDto
    {
        public ProfileLongDto? Profile { get; set; }
        public List<UsersLongDto>? User { get; set; }
        public Friendship? Friendship { get; set; }

    }
}
