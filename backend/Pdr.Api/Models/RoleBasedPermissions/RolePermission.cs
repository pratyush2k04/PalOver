using System.Data;

namespace Pdr.Api.Models.RoleBasedPermissions
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public string Permission { get; set; } = string.Empty;

        // Navigation Property
        public Role Role { get; set; } = null!;
    }
}
