using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Aplication.Interfaces.Services;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System;



namespace Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;

        
        public JwtTokenService(IConfiguration configuration)
        {
            _secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "deault";
            _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "deault";
            _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "deault";
        }




        public string GenerateToken( string profileId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

           
            string encryptedProfileId = Encrypt(profileId);
            var claims = new[]
            {
                
                new Claim("profileID",encryptedProfileId)
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateAdminToken(string adminID)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            string encryptedAdminId = Encrypt(adminID);
            var claims = new[]
            {

                new Claim("adminID",encryptedAdminId)
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var prinsipal = tokenHandler.ValidateToken( token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = securityKey
                }, out var validatedToken);

                return prinsipal;
            }
            catch
            {
                return null;
            }
        }

        public string JWTReader(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(jwt);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        }

        public string GenerateRefeshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        public string? GetAdminIdFromJwt(ClaimsPrincipal claims)
        {
            var claimValue = claims.FindFirst("adminID")?.Value;

            if (string.IsNullOrEmpty(claimValue))
                return null;

            return Decrypt(claimValue);
        }
        public string? GetProfileIdFromJwt(ClaimsPrincipal claims)
        {
            var claimValue = claims.FindFirst("profileID")?.Value;

            if (string.IsNullOrEmpty(claimValue))
                return null;

            return Decrypt(claimValue);
        }

        //funciones internas
        private string Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_secret.Substring(0, 32)); // 32 bytes para AES-256
                aes.IV = new byte[16]; // IV en cero para simplicidad, pero deberías usar un IV seguro

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                return Convert.ToBase64String(encryptedBytes);
            }
        }
        private string Decrypt(string cipherText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_secret.Substring(0, 32));
                aes.IV = new byte[16];

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                var cipherBytes = Convert.FromBase64String(cipherText);
                var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
     
    }
}
