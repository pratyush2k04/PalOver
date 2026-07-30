using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.RoleBasedPermissions
{
    public class EngagementAssignment
    {
        public int UserId { get; set; }
        public int EngagementId { get; set; }

        // Navigation Property
        public User User { get; set; } = null!;
    }
}
