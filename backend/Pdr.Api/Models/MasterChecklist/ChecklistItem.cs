namespace Pdr.Api.Models.MasterChecklist
{
    public class ChecklistItem
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long CategoryId { get; set; }
        public bool IsMandatory { get; set; } = true;
        public int DisplayOrder { get; set; }
        public bool IsRetired { get; set; } = false;

        // Navigation properties
        public ChecklistVersion Version { get; set; } = null!;
        public ChecklistCategory Category { get; set; } = null!;
    }
}
