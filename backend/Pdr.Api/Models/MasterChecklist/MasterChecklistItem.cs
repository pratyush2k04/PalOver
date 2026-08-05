namespace Pdr.Api.Models.MasterChecklist
{
    public class MasterChecklistItem : BaseAuditableEntity
    {
        //tracking changes and supporting soft deletion
        public int Id { get; set; }
        public int VersionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public bool IsMandatory { get; set; } = true;
        public int DisplayOrder { get; set; }
        public bool IsRetired { get; set; } = false;

        // Navigation properties
        public MasterChecklistVersion Version { get; set; } = null!;
        public MasterChecklistCategory Category { get; set; } = null!;
    }
}
