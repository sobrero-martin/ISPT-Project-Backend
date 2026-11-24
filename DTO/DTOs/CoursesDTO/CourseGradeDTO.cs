namespace DTO.DTOs.CoursesDTO;

public class CourseGradeDTO
{
    public long FileDivisionId { get; set; }
    public string FileCode { get; set; } = "";
    public string StudentName { get; set; } = "";
    public short? Grade { get; set; }
}

public class CourseGradeUpdateDTO
{
    public long FileDivisionId { get; set; }
    public short? Grade { get; set; }
}
