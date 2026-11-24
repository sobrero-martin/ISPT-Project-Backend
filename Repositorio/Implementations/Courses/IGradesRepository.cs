using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;

namespace Repositorio.Implementations.Courses;

public interface IGradesRepository
{
    Task<ResponseDTO<List<CourseGradeDTO>>> GetGrades(long evaluationId);
    Task<ResponseDTO<string>> SaveGrades(long evaluationId, List<CourseGradeUpdateDTO> grades);
}
