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
        Task<ResponseDTO<FinalExamPostDTO>> GetFinalExamById(long id);
        Task<ResponseDTO<string>> Post(FinalExamPostDTO exam);
        Task<ResponseDTO<string>> Edit(FinalExamPostDTO exam);
        Task<ResponseDTO<string>> Delete(long id);
    }
}
