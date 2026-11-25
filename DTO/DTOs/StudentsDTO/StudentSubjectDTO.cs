namespace DTO.DTOs.StudentsDTO;

public class StudentSubjectDTO
{

    public long FileDivisionId { get; set; }
    public long DivisionId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public int SchoolYear { get; set; }
    public string Division { get; set; } = string.Empty;
    public double? Average { get; set; }
    public double? AttendancePercentage { get; set; }
    public string Status { get; set; } = string.Empty;
}
