using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using DTO.DTOs.StudentsDTO;
using Repositorio.Repository;

namespace Repositorio.Implementations.Students;

public interface IStudentRepository : IPersonRepository
{
    public Task<ResponseDTO<List<StudentDTO>>> GetAllStudents();
    public Task<ResponseDTO<List<StudentFileDivisionDTO>>> GetStudentsBySchoolYearId(long schoolYearId);
    public Task<ResponseDTO<List<StudentSubjectDTO>>> GetStudentSubjects(long personId);
    public Task<ResponseDTO<List<StudentEvaluationDTO>>> GetStudentEvaluations(long personId, long fileDivisionId);
    public Task<ResponseDTO<List<StudentAttendanceDTO>>> GetStudentAttendance(long personId, long fileDivisionId);
    public Task<ResponseDTO<int>> ImportStudentsFromExcel(Stream fileStream);
}
