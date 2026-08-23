using DTO.ENUM;

namespace DTO.DTOs.StudentsDTO;

public class FileReadDTO
{
    public long Id { get; set; }
    public string Code { get; set; }
    public long CareerId { get; set; }
    public long CurriculumId { get; set; }
    public EnumStudentStatus Status { get; set; }
}