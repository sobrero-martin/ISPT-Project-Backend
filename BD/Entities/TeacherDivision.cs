using DTO.ENUM;

namespace BD.Entidades;

public class TeacherDivision : BaseEntity
{
    public long TeacherId { get; set; }
    public Person Teacher { get; set; }
    
    public long DivisionId { get; set; }
    public Division Division { get; set; }
    
    public EnumTeacherStatus TeacherStatus { get; set; }
    public DateTime StartDate  { get; set; }
    public DateTime EndDate { get; set; }
    
    public string Observations  { get; set; }
}