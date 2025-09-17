using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AlonBuyumEx.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateToken(string userName)
        {
            // a simple identification by name. for a real world scenario, a real user identification system is required
            var tokenHandler = new JsonWebTokenHandler(); /*JwtSecurityTokenHandler()*/
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ?? throw new KeyNotFoundException("JWT secret is not configured"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Name, userName),
                      new Claim(ClaimTypes.Role, userName == "Admin" ? "Admin" : "User") // simple role assignment based on userName
                  ]),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256/*HmacSha256Signature*/),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            /*return tokenHandler.WriteToken(token);*/
            return token;
        }
    }
}
