using DTO.DTOs.DTO_Response;
using DTO.DTOs.PersonDTO;
using DTO.DTOs.SchoolYearDTO;
using DTO.DTOs.StudentsDTO;

namespace Repositorio.Repository;

public interface IStudentRepository : IPersonRepository
{
    public Task<ResponseDTO<List<StudentDTO>>> GetAllStudents();
    public Task<ResponseDTO<List<StudentFileDivisionDTO>>> GetStudentsBySchoolYearId(long schoolYearId);
    public Task<ResponseDTO<int>> ImportStudentsFromExcel(Stream fileStream);
}