using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations.Careers
{
    public interface IDivisionTemplateRepository
    {
        Task<ResponseDTO<List<DivisionTemplateDTO>>> GetBySubject(long subjectId);
        Task<ResponseDTO<DivisionTemplateDTO>> Post(long subjectId);
    }
}
