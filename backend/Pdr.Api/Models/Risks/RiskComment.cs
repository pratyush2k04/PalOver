using Pdr.Api.Models.LoginEntity;

namespace Pdr.Api.Models.Risks;

public class RiskComment : BaseAuditableEntity
{
    public int Id { get; set; }

    public int RiskId { get; set; }


    public string Content { get; set; } = string.Empty;

    public bool IsEdited { get; set; } = false;
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
}
