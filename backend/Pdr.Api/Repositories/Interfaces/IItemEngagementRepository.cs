using System.Threading.Tasks;
using Pdr.Api.Models;

namespace Pdr.Api.Repositories.Interfaces;

public interface IItemEngagementRepository
{
    Task<int?> GetEngagementIdAsync(ItemEntityType entityType, int itemId);
}
