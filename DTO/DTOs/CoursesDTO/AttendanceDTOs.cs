namespace DTO.DTOs.CoursesDTO;

public class AttendanceDayDTO
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public int Present { get; set; }
    public int Total { get; set; }
    public string State { get; set; } = "Pendiente";
}

public class DailyAttendanceDTO
{
    public long FileDivisionId { get; set; }
    public string FileCode { get; set; } = "";
    public string StudentName { get; set; } = "";
    public string Status { get; set; } = "Pendiente";
}

public class AttendanceUpdateDTO
{
    public long FileDivisionId { get; set; }
    public string Status { get; set; } = "";
}
