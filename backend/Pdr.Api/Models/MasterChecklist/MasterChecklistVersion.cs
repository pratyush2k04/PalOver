namespace Pdr.Api.Models.MasterChecklist
{
    public class MasterChecklistVersion : BaseAuditableEntity
    {
        //have audit timestamps for when draft versions are created or updated
        public int Id { get; set; }
        public int VersionNumber { get; set; }
        public int StatusId { get; set; }

        // Navigation properties
        public MasterChecklistStatus Status { get; set; } = null!;
        public ICollection<MasterChecklistItem> Items { get; set; } = new List<MasterChecklistItem>();
    }
}
