namespace Pdr.Api.Models.Documents;

public class DocumentCategory
{
    public int Id { get; set; }

    public required string CategoryName { get; set; }

    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
