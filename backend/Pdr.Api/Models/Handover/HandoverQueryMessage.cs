using System.Text.Json.Serialization;
using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.Handover;

public class HandoverQueryMessage : BaseAuditableEntity
{
    public int Id { get; set; }

    public int QueryId { get; set; }

    public int SenderUserId { get; set; }

    public HandoverQueryMessageType MessageType { get; set; }

    public string MessageText { get; set; } = string.Empty;

    public int SequenceNumber { get; set; } = 1;

    [JsonIgnore]
    public HandoverQuery Query { get; set; } = null!;

    public User SenderUser { get; set; } = null!;
}
