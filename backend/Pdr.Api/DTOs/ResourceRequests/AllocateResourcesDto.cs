using System.Collections.Generic;

namespace Pdr.Api.DTOs.ResourceRequests;

public class AllocateResourcesDto
{
    public List<string> Names { get; set; } = [];
}
