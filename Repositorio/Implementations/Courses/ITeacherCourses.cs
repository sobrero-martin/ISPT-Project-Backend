using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;

namespace Repositorio.Repository;

public interface ITeacherCourses
{
    Task<ResponseDTO<List<TeacherCoursesDTO>>> GetCoursesByTeacherId(long teacherId);
    Task<ResponseDTO<List<ScheduleDTO>>> GetScheduleByCourseId(long id);
}