namespace Pdr.Api.DTOs.Common;

public class CursorPagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int? NextCursor { get; set; }
    public bool HasMore { get; set; }
}
