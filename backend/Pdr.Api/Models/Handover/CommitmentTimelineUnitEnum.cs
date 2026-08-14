using System.Text.Json.Serialization;

namespace Pdr.Api.Models.Handover;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommitmentTimelineUnitEnum
{
    Days = 1,
    Weeks = 2,
    Months = 3
}
