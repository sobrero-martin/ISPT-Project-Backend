using BD.Entidades;
using DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Repository
{
    public interface ISubjectRepository
    {
        Task<List<SubjectDTO>> GetByCurriculum(long curriculumId);
        Task<SubjectDTO> GetById (long id);
        Task<bool> Put(long id, Subject subject);
        Task<SubjectDTO> Post(Subject subject);
    }
}
