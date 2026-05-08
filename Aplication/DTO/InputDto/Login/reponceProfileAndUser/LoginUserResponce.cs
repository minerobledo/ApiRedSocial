using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO.InputDto.Login.reponceProfileAndUser
{
    public class LoginUserResponce
    {
        public string? Id { get; set; }
        public string? Nickname { get; set; }
        public string? Email { get; set; }
        public DateTime? birthdate { get; set; }
        public string? Gender { get; set; }
        public string? Orientation { get; set; }
        public string? Traits { get; set; }
        public string? Province { get; set; }
        public string? Height { get; set; }
        public string? Weight { get; set; }
        public string? ZodiacSign { get; set; }
        public string? EyeColor { get; set; }
        public string? HairType { get; set; }
        public string? Shaved { get; set; }
        public string? EducationLevel { get; set; }
    }
}
