namespace Pdr.Api.Models.MasterChecklist
{
    public class ChecklistVersion
    {
        public long Id { get; set; }
        public int VersionNumber { get; set; }
        public long StatusId { get; set; }
        public long CreatedBy { get; set; }

        // Navigation properties
        public VersionStatus Status { get; set; } = null!;
        public ICollection<ChecklistItem> Items { get; set; } = new List<ChecklistItem>();
    }
}
