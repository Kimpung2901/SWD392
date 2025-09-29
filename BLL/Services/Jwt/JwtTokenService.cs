using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DAL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Services.Jwt
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;
        public JwtTokenService(IConfiguration config) => _config = config;

        public string CreateAccessToken(User user, TimeSpan lifetime)
        {
            var issuer = _config["Jwt:Issuer"] ?? throw new Exception("Missing Jwt:Issuer");
            var audience = _config["Jwt:Audience"] ?? throw new Exception("Missing Jwt:Audience");
            var keyBytes = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new Exception("Missing Jwt:Key"));

            var claims = new List<Claim>
            {

                new(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new(ClaimTypes.NameIdentifier, user.UserID.ToString()),

                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, user.Role ?? "User"),

                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(lifetime),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static TokenValidationParameters GetValidationParameters(IConfiguration cfg)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = cfg["Jwt:Issuer"],
                ValidAudience = cfg["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!)),
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
