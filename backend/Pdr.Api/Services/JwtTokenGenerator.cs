using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Pdr.Api.Constants;
using Pdr.Api.Models.LoginEntity;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    public string GenerateToken(User user)
    {
        var secretKey = configuration[AuthConstants.ConfigKeys.SecretKey] 
            ?? throw new InvalidOperationException(AuthConstants.Messages.SecretKeyMissing);
        var issuer = configuration[AuthConstants.ConfigKeys.Issuer];
        var audience = configuration[AuthConstants.ConfigKeys.Audience];
        var expiryMinutes = int.TryParse(configuration[AuthConstants.ConfigKeys.ExpiryMinutes], out var mins) ? mins : AuthConstants.DefaultExpiryMinutes;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.Name), // For ASP.NET Core [Authorize(Roles = "...")]
            new Claim(ClaimConstants.Role, user.Role.Name),           // Clean key for Angular frontend
            new Claim(ClaimConstants.RoleId, user.RoleId.ToString()),
            new Claim(ClaimConstants.RoleIdSnake, user.RoleId.ToString()),
            new Claim(ClaimConstants.Email, user.Email),
            new Claim(ClaimConstants.FullName, user.FullName),
            new Claim(ClaimConstants.IsAdmin, user.IsAdmin.ToString().ToLower())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
