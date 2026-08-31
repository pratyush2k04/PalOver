using System.Text.Json.Serialization;

namespace Pdr.Api.Models.Handover;

public class HandoverQueryResponse : BaseAuditableEntity
{
    public int QueryId { get; set; }

    public string ResponseText { get; set; } = string.Empty;

    [JsonIgnore]
    public HandoverQuery Query { get; set; } = null!;
}
