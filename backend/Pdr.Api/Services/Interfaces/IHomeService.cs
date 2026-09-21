using Pdr.Api.DTOs.Home;

namespace Pdr.Api.Services.Interfaces;

public interface IHomeService
{
    Task<HomeResponseDto> GetHomeAsync();
}
