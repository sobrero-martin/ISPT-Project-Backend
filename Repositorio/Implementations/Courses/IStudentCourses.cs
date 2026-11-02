using DTO.DTOs.CoursesDTO;
using DTO.DTOs.CoursesDTO.InscriptionForm;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;

namespace Repositorio.Implementations.Students;

public interface IStudentCourses
{
    Task<ResponseDTO<List<CareerInscriptionDTO>>> GetAllAvailableCareerInscription();
    Task<ResponseDTO<InscriptionFormDTO>> GetAllSubjectsFromCareer(long curriculumId, int schoolYear, long studentId);
    Task<ResponseDTO<List<ScheduleInscriptionDTO>>> GetAllDivisionSchedules(List<long> divisionIds);
    Task<ResponseDTO<string>> Inscription();
    Task GetCoursesByStudentId(long studentId);
}