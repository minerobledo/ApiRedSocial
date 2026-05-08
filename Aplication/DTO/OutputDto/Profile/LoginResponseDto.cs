using Aplication.DTO.InputDto.Login.reponceProfileAndUser;
using Aplication.ResponPattern;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.OutputDto.Profile
{
    public class LoginResponseDto
    {
        public string? JWT { get; set; }
        public string? RefreshToken { get; set; }
        public SelfProfile? SelfProfile { get; set; }
    }
}
