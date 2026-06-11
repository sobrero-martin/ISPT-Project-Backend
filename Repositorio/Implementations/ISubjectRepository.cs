using BD.Entidades;
using DTO.DTOs.CareerDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations
{
    public interface ISubjectRepository
    {
        Task<List<SubjectDTO>> GetByCurriculum(long curriculumId);
        Task<SubjectDTO> GetById(long id);
        Task<bool> Put(long id, Subject subject);
        Task<SubjectDTO> Post(Subject subject);
        Task<List<SubjectDTO>> GetPossibleCorrelatives(long curriculumId, long subjectId);

    }
}
