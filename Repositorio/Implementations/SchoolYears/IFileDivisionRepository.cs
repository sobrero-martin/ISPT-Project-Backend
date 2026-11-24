using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations.SchoolYears
{
    public interface IFileDivisionRepository
    {
        Task<ResponseDTO<List<FileDivisionDTO>>> GetByDivisionId(long divisionId);
        Task<ResponseDTO<string>> Post(StudentFileDivisionPostDTO studentFileDivisionDTO);
        Task<ResponseDTO<List<InscriptionRequestDTO>>> GetInscriptionRequests(long schoolYearId);
        Task<ResponseDTO<string>> ReviewInscriptionRequests(long schoolYearId, ReviewInscriptionRequestsDTO review);
    }
}
