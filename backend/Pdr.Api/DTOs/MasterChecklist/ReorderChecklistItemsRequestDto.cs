namespace Pdr.Api.DTOs.MasterChecklist;

public class ReorderChecklistItemsRequestDto
{
    public required List<ChecklistItemOrderDto> Items { get; set; }
}

public class ChecklistItemOrderDto
{
    public long ItemId { get; set; }

    public int DisplayOrder { get; set; }
}
