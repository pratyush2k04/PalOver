using System.Text.Json.Serialization;

namespace Pdr.Api.Models.People;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PersonType
{
    Internal = 1,
    Client = 2
}
