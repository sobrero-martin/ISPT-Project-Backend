using DTO.DTOs.DTO_Response;
using DTO.DTOs.ExamDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations.Exams
{
    public interface IFinalExamRepository
    {
        Task<ResponseDTO<List<FinalExamDTO>>> GetFull();
        Task<ResponseDTO<string>> Post(FinalExamPostDTO exam);
    }
}
