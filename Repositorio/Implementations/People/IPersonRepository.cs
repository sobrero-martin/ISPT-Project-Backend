using DTO.DTOs.DTO_Response;
using DTO.DTOs.PersonDTO;

namespace Repositorio.Repository;

public interface IPersonRepository
{
    public Task<ResponseDTO<PersonDTO>> GetPerson(long id);
    public Task<ResponseDTO<ContactDTO>> GetContactByPersonId(long id);
    public Task<ResponseDTO<ObservationDTO>> GetObservationByPersonId(long id);
    public Task<ResponseDTO<string>> AddPerson(PersonDTO personDTO); 
    public Task<ResponseDTO<string>> EditPerson(PersonDTO personDTO); 
    public Task<ResponseDTO<string>> EditContact(ContactDTO contactDTO);
    public Task<ResponseDTO<string>> EditObservation(ObservationDTO observationDTO);
}