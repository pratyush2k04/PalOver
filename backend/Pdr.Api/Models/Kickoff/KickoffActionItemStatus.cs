using System.Text.Json.Serialization;

namespace Pdr.Api.Models.Kickoff;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum KickoffActionItemStatus
{
    InProgress = 1,
    Done = 2
}
