using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations.Careers
{
    public interface ISubjectRepository
    {
        Task<ResponseDTO<List<SubjectTableDTO>>> GetByCurriculum(long curriculumId);
        Task<ResponseDTO<SubjectDTO>> GetById(long id);
        Task<ResponseDTO<string>> Put(long id, SubjectPostDTO subject);
        Task<ResponseDTO<string>> Post(SubjectPostDTO subject);
        Task<ResponseDTO<List<SubjectCorrelativesDTO>>> GetPossibleCorrelatives(long curriculumId, long subjectId);
        Task<ResponseDTO<List<SubjectTableDTO>>> GetBySchoolYear(long schoolYearId);
    }
}
