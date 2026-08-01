namespace DTO.DTOs.TeachersDTO;

public class TeacherDivisionDTO
{
    public long Id { get; set; }
    public string DNI { get; set; }
    public string TeacherFullname { get; set; }
    public string TeacherStatus  { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}