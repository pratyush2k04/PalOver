using System.Text.Json.Serialization;

namespace Pdr.Api.Models.Risks;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RiskEscalationLevel
{
    PM = 1,
    Leadership = 2
}
