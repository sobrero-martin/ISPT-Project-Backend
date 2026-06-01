namespace BD.Entidades;

public class TeacherDivision : BaseEntity
{
    public long TeacherId { get; set; }
    public Person Teacher { get; set; }
    
    public long DivisionId { get; set; }
    public Division Division { get; set; }
    
    
}