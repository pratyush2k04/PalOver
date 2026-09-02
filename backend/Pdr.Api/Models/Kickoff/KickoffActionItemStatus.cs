using System.Text.Json.Serialization;

namespace Pdr.Api.Models.Kickoff;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum KickoffActionItemStatus
{
    InProgress = 2,
    Done = 3
}
