using DTO.DTOs.DTO_Response;
using DTO.DTOs.TeachersDTO;

namespace Repositorio.Repository;

public interface ITeacherRepository : IPersonRepository
{
    public Task<ResponseDTO<List<TeacherDTO>>> GetAllTeachers();
}