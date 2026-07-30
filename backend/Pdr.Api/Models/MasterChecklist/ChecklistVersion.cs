using Microsoft.AspNetCore.Http.HttpResults;

namespace Pdr.Api.Models.MasterChecklist
{
    public class ChecklistVersion : BaseAuditableEntity
    {
        //have audit timestamps for when draft versions are created or updated
        public long Id { get; set; }
        public int VersionNumber { get; set; }
        public long StatusId { get; set; }
        //public long CreatedBy { get; set; }

        // Navigation properties
        public ChecklistStatus Status { get; set; } = null!;
        public ICollection<ChecklistItem> Items { get; set; } = new List<ChecklistItem>();
    }
}
