using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations.Courses
{
    public interface IDivisionExamRepository
    {
        Task<ResponseDTO<List<DivisionExamReadDTO>>> GetAllEvaluations(long divisionId);
        Task<ResponseDTO<string>> CreateEvaluation(DivisionExamPostDTO divisionExamDTO);
        Task<ResponseDTO<string>> DeleteEvaluation(long evaluationId);
    }
}
