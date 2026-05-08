using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string profileId);
        string GenerateAdminToken(string adminID);
        string GenerateRefeshToken();
        ClaimsPrincipal? ValidateToken(string token);
        string JWTReader(string jwt);

        string? GetAdminIdFromJwt(ClaimsPrincipal claims);
        string? GetProfileIdFromJwt(ClaimsPrincipal claims);
    }
}
