namespace DTO.DTOs.Documentation;

public class FileDocumentationRowPerYear
{
    public long Id { get; set; }
    public long FileId { get; set; }
    public Guid? UpdatedById { get; set; }
    public bool CUS { get; set; }
    public bool CNIRDS { get; set; }
    public bool CDA { get; set; }
    public bool Cooperative  { get; set; }
    public int Date {get; set;}
}