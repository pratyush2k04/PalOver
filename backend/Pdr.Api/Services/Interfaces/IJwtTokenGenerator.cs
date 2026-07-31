using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Services.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
