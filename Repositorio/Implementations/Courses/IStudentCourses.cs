using DTO.DTOs.CoursesDTO;
using DTO.DTOs.CoursesDTO.InscriptionForm;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;

namespace Repositorio.Implementations.Students;

public interface IStudentCourses
{
    Task<ResponseDTO<List<CareerInscriptionDTO>>> GetAllAvailableCareerInscription(long studentId);
    Task<ResponseDTO<InscriptionFormDTO>> GetAllSubjectsFromCareer(long curriculumId, int schoolYear, long studentId);
    Task<ResponseDTO<string>> Inscription(StudentInscriptionDTO inscription, long studentId);
    Task<ResponseDTO<List<ScheduleInscriptionDTO>>> GetAllDivisionSchedules(List<long> divisionIds);
    Task GetCoursesByStudentId(long studentId);
}
