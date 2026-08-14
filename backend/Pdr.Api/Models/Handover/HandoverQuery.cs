using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.Handover;

public class HandoverQuery : BaseAuditableEntity
{
    public int QueryId { get; set; }

    public int HandoverId { get; set; }

    public int FieldSectionId { get; set; }

    public int? ReferenceId { get; set; }

    public bool IsResolved { get; set; } = false;

    public int? ResolvedByUserId { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public HandoverQueryFieldSection FieldSection { get; set; } = null!;

    public User? ResolvedByUser { get; set; }

    public HandoverQueryQuestion? Question { get; set; }

    public HandoverQueryResponse? Response { get; set; }
}
