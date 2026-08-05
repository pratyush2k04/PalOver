namespace Pdr.Api.DTOs.MasterChecklist;

public class ReorderMasterChecklistItemsRequestDto
{
    public required List<ChecklistItemOrderDto> Items { get; set; }
}

public class ChecklistItemOrderDto
{
    public int ItemId { get; set; }

    public int DisplayOrder { get; set; }
}
