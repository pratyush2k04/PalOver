using Microsoft.AspNetCore.Http.HttpResults;

namespace Pdr.Api.Models.MasterChecklist
{
    public class MasterChecklistVersion : BaseAuditableEntity
    {
        //have audit timestamps for when draft versions are created or updated
        public long Id { get; set; }
        public int VersionNumber { get; set; }
        public long StatusId { get; set; }
        //public long CreatedBy { get; set; }

        // Navigation properties
        public MasterChecklistStatus Status { get; set; } = null!;
        public ICollection<MasterChecklistItem> Items { get; set; } = new List<MasterChecklistItem>();
    }
}
