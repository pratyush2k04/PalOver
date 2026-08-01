using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Pdr.Api.Services.Interfaces;

namespace Pdr.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int GetUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;

        var subClaim = user?.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? user?.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("No authenticated user found on the current request.");

        return int.Parse(subClaim.Value);
    }
}
