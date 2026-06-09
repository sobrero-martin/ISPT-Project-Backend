using DTO.DTOs.DTO_Response;
using DTO.DTOs.PersonDTO;

namespace Repositorio.Repository;

public interface IPersonRepository
{
    public Task<ResponseDTO<List<PersonDTO>>> GetAllPeopleByRol();
    public Task<ResponseDTO<PersonDTO>> GetPerson(long id);
    public Task<ResponseDTO<string>> AddPerson(PersonDTO personDTO); 
    public Task<ResponseDTO<string>> EditPerson(PersonDTO personDTO); 
}