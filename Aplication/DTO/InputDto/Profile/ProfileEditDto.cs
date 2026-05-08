using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Profile
{
    public class ProfileEditDto
    {
        
        

        public string? Id { get; set; }
        public int User { get; set; }
        public bool GetOut { get; set; }
        public IFormFile? ProfilePhoto { get; set; } = null;
        public IFormFile? CoverPhoto { get; set; } = null;
        public string? Description { get; set; } = null;
        //user
        public string? Nickname { get; set; } = null;
        public string? Gender { get; set; } = null;
        public string? Orientation { get; set; } = null;
        public string? Traits { get; set; } = null;
        public string? Province { get; set; } = null;
        public string? Height { get; set; } = null;
        public string? Weight { get; set; } = null;
        public string? ZodiacSign { get; set; } = null;
        public string? EyeColor { get; set; } = null;
        public string? HairType { get; set; } = null;
        public string? Shaved { get; set; } = null;
        public string? EducationLevel { get; set; } = null;
    }
}

