using Aplication.ResponPattern;
using Google.Cloud.Firestore;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Register
{
    public class RecuestRegisterDto
    {
        public List<RegisterUserDto>? registerUserDtos { get; set; }
        public RegisterProfileDto? profileDtos { get; set; }
    }
}
