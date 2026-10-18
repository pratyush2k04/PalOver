namespace Pdr.Api.Models.AccessTracker
{
    public class AccessItemDefinition 
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsClientSide { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<AccessItem> AccessItems { get; set; }
            = new List<AccessItem>();
    }
}
