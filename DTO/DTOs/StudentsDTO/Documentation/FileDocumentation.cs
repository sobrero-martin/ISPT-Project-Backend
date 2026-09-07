namespace DTO.DTOs.Documentation;

public class FileDocumentation
{
    public long FileId { get; set; }
    public Guid? UpdatedById { get; set; }
    public bool DNI { get; set; }
    public bool Picture { get; set; }
    public bool BirthdateDocument { get; set; }
    public List<FileDocumentationRowPerYear> Rows { get; set; } = [];
}