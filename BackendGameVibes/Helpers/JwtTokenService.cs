using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendGameVibes.Helpers {
    public class JwtTokenService {

        private readonly IConfiguration _configuration;
        SymmetricSecurityKey key;
        string issuer;
        string audience;

        public JwtTokenService(IConfiguration configuration) {
            _configuration = configuration;
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            issuer = _configuration["Jwt:Issuer"]!;
            audience = _configuration["Jwt:Audience"]!;
        }

        public string GenerateToken(string email, string username, string userId, string role) {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
             };

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string? email, string? username, string? userId, string? role) GetTokenClaims(string token) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = key
            };

            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

            if (principal != null) {
                var email = principal.FindFirst(ClaimTypes.Email)!.Value ?? null;
                var username = principal.FindFirst(ClaimTypes.Name)!.Value ?? null;
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)!.Value ?? null;
                var role = principal.FindFirst(ClaimTypes.Role)!.Value ?? null;
                return (email, username, userId, role);
            }

            return (null, null, null, null);
        }
    }
}
