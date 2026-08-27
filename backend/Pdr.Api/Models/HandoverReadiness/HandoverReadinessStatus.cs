using System.Text.Json.Serialization;

namespace Pdr.Api.Models.HandoverReadiness;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HandoverReadinessStatus
{
    InProgress = 1,
    Granted = 2,
    Blocked = 3
}
