using System.Text.Json.Serialization;

namespace Pdr.Api.Models.Engagements.Dependencies;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DependencyType
{
    Internal = 1,
    External = 2
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DependencyStatus
{
    Open = 1,
    Met = 2,
    AtRisk = 3
}
