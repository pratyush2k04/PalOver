using System.Text.Json.Serialization;

namespace Pdr.Api.Models.Handover;

public class HandoverQueryQuestion : BaseAuditableEntity
{
    public int QueryId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    [JsonIgnore]
    public HandoverQuery Query { get; set; } = null!;
}
