using DTO.ENUM;

namespace DTO.DTOs.StudentsDTO;

public class FilePostDTO
{
    public long? Id { get; set; }
    public Guid? CreatedById { get; set; }
    public Guid? UpdatedById { get; set; }
    
    public string Code { get; set; }
    public long StudentId { get; set; }
    public long CurriculumId { get; set; }
    public EnumStudentStatus Status { get; set; }
}