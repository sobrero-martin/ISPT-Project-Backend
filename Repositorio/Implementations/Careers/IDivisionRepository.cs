using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations.Careers
{
    public interface IDivisionRepository
    {
        Task<ResponseDTO<List<DivisionDTO>>> GetBySchoolYearSubject(long schoolYearId, long subjectId);
    }
}
