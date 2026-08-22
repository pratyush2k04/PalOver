using System.Text.Json.Serialization;

namespace Pdr.Api.Models.Engagements;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EvidenceSnapshotItemStatus
{
    Open = 1,
    Complete = 2,
    NotApplicable = 3
}
