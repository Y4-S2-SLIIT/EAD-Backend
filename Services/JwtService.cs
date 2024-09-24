using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EADBackend.Services
{
    public interface IJwtService
    {
        string GenerateToken(string userId);
    }

    public class JwtService(IConfiguration configuration) : IJwtService
    {
        private readonly string _secret = configuration["JwtSettings:Secret"]
                ?? throw new ArgumentNullException(nameof(configuration), "JwtSettings:Secret is not configured.");
        private readonly string _issuer = configuration["JwtSettings:Issuer"]
                ?? throw new ArgumentNullException(nameof(configuration), "JwtSettings:Issuer is not configured.");
        private readonly string _audience = configuration["JwtSettings:Audience"]
                ?? throw new ArgumentNullException(nameof(configuration), "JwtSettings:Audience is not configured.");
        private readonly int _expirationInMinutes = int.TryParse(configuration["JwtSettings:ExpirationInMinutes"], out var expiration)
                ? expiration
                : throw new ArgumentNullException(nameof(configuration), "JwtSettings:ExpirationInMinutes is not configured or invalid.");

        public string GenerateToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            // Create token with userId as NameIdentifier
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId) // UserId claim
                }),
                Expires = DateTime.UtcNow.AddMinutes(_expirationInMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}