using DTO.DTOs.DTO_Response;
using DTO.DTOs.StudentsDTO;

namespace Repositorio.Repository;

public interface IDegreesRepository
{
    public Task<ResponseDTO<List<DegreeDTO>>> GetDegreesByPersonId(long id);
    public Task<ResponseDTO<string>> createDegree(DegreeDTO degreeDTO);
    public Task<ResponseDTO<string>> deleteDegree(long id);
}