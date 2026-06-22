using DTO.DTOs.DTO_Response;
using DTO.DTOs.PersonDTO;
using DTO.DTOs.StudentsDTO;

namespace Repositorio.Repository;

public interface IStudentRepository : IPersonRepository
{
    public Task<ResponseDTO<List<StudentDTO>>> GetAllStudents();
}