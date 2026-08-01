using Pdr.Api.Models;

namespace Pdr.Api.Repositories.Interfaces;

public interface IEngagementRepository
{
    Task<List<Engagement>> GetEngagementsForPmAsync(int userId, string sortBy);
}
