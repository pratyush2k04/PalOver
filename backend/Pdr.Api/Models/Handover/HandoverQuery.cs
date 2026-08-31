using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.Handover;

public class HandoverQuery : BaseAuditableEntity
{
    public int Id { get; set; }

    public int HandoverId { get; set; }

    public int FieldSectionId { get; set; }

    public int? ReferenceId { get; set; }

    public bool IsResolved { get; set; } = false;

    public int RaisedByUserId { get; set; }

    public int? RespondedByUserId { get; set; }

    public int? ResolvedByUserId { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public HandoverQueryFieldSection FieldSection { get; set; } = null!;

    public User RaisedByUser { get; set; } = null!;

    public User? RespondedByUser { get; set; }

    public User? ResolvedByUser { get; set; }

    public HandoverQueryQuestion? Question { get; set; }

    public HandoverQueryResponse? Response { get; set; }
}
