using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations.SchoolYears
{
    public interface IScheduleRepository
    {
        Task<ResponseDTO<List<ScheduleDTO>>> GetByDivisionId(long divisionId);
    }
}
