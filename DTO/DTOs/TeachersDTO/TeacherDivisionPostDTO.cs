using DTO.ENUM;

namespace DTO.DTOs.TeachersDTO;

public class TeacherDivisionPostDTO
{
    public long? Id { get; set; }
    public Guid? CreatedById { get; set; }
    public Guid? UpdatedById { get; set; }
    
    public long TeacherId { get; set; }
    public long DivisionId { get; set; }
    
    public EnumTeacherStatus TeacherStatus { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Observations  { get; set; }
}