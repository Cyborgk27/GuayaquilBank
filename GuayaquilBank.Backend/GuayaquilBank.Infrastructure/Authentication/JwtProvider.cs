using GuayaquilBank.Domain.Entities.Identity;
using GuayaquilBank.Domain.Interfaces;
using GuayaquilBank.Infrastructure.Common.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GuayaquilBank.Infrastructure.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        private readonly InfrastructureSettings _settings;

        public JwtProvider(IOptions<InfrastructureSettings> settings)
        {
            _settings = settings.Value;
        }
        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userName", user.Username),
                new Claim("companyId", user.CompanyId.ToString() ?? ""),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Security.Jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Security.Jwt.Issuer,
                audience: _settings.Security.Jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.Security.Jwt.DurationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
