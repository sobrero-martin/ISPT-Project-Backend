using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;

namespace Repositorio.Repository;

public interface ITeacherCourses
{
    Task<ResponseDTO<List<TeacherCoursesDTO>>> GetCoursesByTeacherId(long teacherId);
}