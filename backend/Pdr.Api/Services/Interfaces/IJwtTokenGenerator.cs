using Pdr.Api.Models;

namespace Pdr.Api.Services.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
