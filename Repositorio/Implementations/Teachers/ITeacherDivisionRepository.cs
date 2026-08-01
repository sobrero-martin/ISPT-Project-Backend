using DTO.DTOs.DTO_Response;
using DTO.DTOs.TeachersDTO;

namespace Repositorio.Repository;

public interface ITeacherDivisionRepository
{
    Task<ResponseDTO<List<TeacherDivisionDTO>>> GetAllTeachersByDivisionId(long divisionId);
    Task<ResponseDTO<TeacherDivisionPostDTO>> GetTeacherDivision(long teacherDivisionId);
    public Task<ResponseDTO<TeacherDivisionObservationDTO>> GetObservationByTeacherDivisionId(long id);
    Task<ResponseDTO<string>> AsignTeacherIntoDivision(TeacherDivisionPostDTO teacherDivisionPostDTO);
    Task<ResponseDTO<string>> UpdateAsignationInDivision(TeacherDivisionPostDTO teacherDivisionPutDTO);
    public Task<ResponseDTO<string>> EditObservation(TeacherDivisionObservationDTO observationDTO);
}