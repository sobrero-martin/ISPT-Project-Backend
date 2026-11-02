namespace DTO.DTOs.StudentsDTO;

public class StudentSubjectDTO
{
    public string Subject { get; set; } = string.Empty;
    public int SchoolYear { get; set; }
    public string Division { get; set; } = string.Empty;
    public double? Average { get; set; }
    public double? AttendancePercentage { get; set; }
}
