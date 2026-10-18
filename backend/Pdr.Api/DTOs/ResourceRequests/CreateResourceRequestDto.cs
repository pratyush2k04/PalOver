using System.Collections.Generic;
using Pdr.Api.Models.ResourceRequests;

namespace Pdr.Api.DTOs.ResourceRequests;

public class CreateResourceRequestDto
{
    public StandardRoleEnum StandardRole { get; set; }
    public HashSet<SkillEnum> Skills { get; set; } = [];
    public int PeopleNeeded { get; set; }
    public DateOnly RequiredByDate { get; set; }
    public string JobDescription { get; set; } = string.Empty;
}
