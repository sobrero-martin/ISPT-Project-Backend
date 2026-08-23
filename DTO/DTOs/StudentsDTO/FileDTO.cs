using DTO.ENUM;

namespace DTO.DTOs.StudentsDTO;

public class FileDTO
{
    public long Id { get; set; }
    public string Code { get; set; }
    public string Career { get; set; }
    public string Curriculum { get; set; }
    public long CurriculumId { get; set; }
    public string Status { get; set; }
}