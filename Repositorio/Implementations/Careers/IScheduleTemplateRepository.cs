using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Implementations.Careers
{
    public interface IScheduleTemplateRepository
    {
        Task<ResponseDTO<List<ScheduleTemplateDTO>>> GetByDivisionTemplateId(long divisionTemplateId);
        Task<ResponseDTO<string>> Post(long divisionTemplateId, ScheduleTemplatePostDTO scheduleTemplateDTO);
        Task<ResponseDTO<ScheduleTemplateDTO>> GetById(long scheduleTemplateId);
    }
}
