using DTO.DTOs.DTO_Response;
using DTO.DTOs.PersonDTO;
using DTO.DTOs.PositionsDTO;

namespace Repositorio.Repository.Positions;

public interface IPositionRepository : IPersonRepository
{
    public Task<ResponseDTO<List<PositionDTO>>> GetAllPersonal();
    public Task<ResponseDTO<PersonDTO>> GetPositionById(long id);
    public Task<ResponseDTO<string>> RemovePersonWithPosition(long id);
}