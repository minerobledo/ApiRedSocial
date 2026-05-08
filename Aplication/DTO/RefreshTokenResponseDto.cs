using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.DTO
{
    public class RefreshTokenResponseDto
    {
        public string? JWT { get; set; }
        public string? RefreshToken;
    }
}
