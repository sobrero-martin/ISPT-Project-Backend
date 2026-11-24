namespace DTO.DTOs.ExamDTO;

public class IEFIDivisionStudentDTO
{
    public long FileDivisionId { get; set; }
    public string FileCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public double? Average { get; set; }
    public double? AttendancePercentage { get; set; }
    public bool Eligible { get; set; }
    public bool Registered { get; set; }
    public int? Grade { get; set; }
    public string? GradeState { get; set; }
}
