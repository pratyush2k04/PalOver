namespace Pdr.Api.DTOs.Engagements;

public class AssignLeadershipUserRequest
{
    public int UserId { get; set; }
    public List<int>? UserIds { get; set; }
}
