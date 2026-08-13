using System.Text.Json.Serialization;

namespace Pdr.Api.Models.Risks;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RiskImpact
{
    Low = 1,
    Medium = 2,
    High = 3
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RiskLikelihood
{
    Low = 1,
    Medium = 2,
    High = 3
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RiskStatus
{
    Open = 1,
    Mitigating = 2,
    Closed = 3
}
