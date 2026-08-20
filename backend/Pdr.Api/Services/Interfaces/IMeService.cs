using Pdr.Api.DTOs.Me;

namespace Pdr.Api.Services.Interfaces;

public interface IMeService
{
    Task<MeDto> GetMeAsync();
    Task<MeDto> UpdateMeAsync(UpdateMeDto request);
    Task ChangePasswordAsync(ChangePasswordDto request);
}
