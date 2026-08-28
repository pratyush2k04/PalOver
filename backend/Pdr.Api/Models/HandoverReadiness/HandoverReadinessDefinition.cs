using System.Collections.Generic;

namespace Pdr.Api.Models.HandoverReadiness;

public class HandoverReadinessDefinition
{
    public int Id { get; set; }
    public string Area { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<HandoverReadinessItem> HandoverReadinessItems { get; set; } = new List<HandoverReadinessItem>();
}
