using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations
{
    public interface ISubjectRepository
    {
        Task<ResponseDTO<List<SubjectDTO>>> GetByCurriculum(long curriculumId);
        Task<ResponseDTO<SubjectDTO>> GetById(long id);
        Task<ResponseDTO<string>> Put(long id, Subject subject);
        Task<ResponseDTO<SubjectDTO>> Post(Subject subject);
        Task<ResponseDTO<List<SubjectDTO>>> GetPossibleCorrelatives(long curriculumId, long subjectId);

    }
}
