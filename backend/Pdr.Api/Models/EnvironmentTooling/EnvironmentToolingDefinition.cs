namespace Pdr.Api.Models.EnvironmentTooling
{
    public class EnvironmentToolingDefinition 
    {
        public int EnvironmentToolingDefinitionId { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsMandatory { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<EnvironmentToolingItem> EnvironmentToolingItems { get; set; }
            = new List<EnvironmentToolingItem>();
    }
}
