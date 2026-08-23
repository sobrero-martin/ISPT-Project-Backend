using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Careers;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api-v1/schedule-templates")]
    public class ScheduleTemplateController : ControllerBase
    {
        private readonly IScheduleTemplateRepository scheduleTemplateRepository;

        public ScheduleTemplateController(IScheduleTemplateRepository scheduleTemplateRepository)
        {
            this.scheduleTemplateRepository = scheduleTemplateRepository;
        }

        [HttpGet("{divisionTemplateId:long}")]
        [Authorize(Roles = "Directivo,Preceptor")]
        public async Task<ActionResult<ResponseDTO<List<ScheduleTemplateDTO>>>> GetByDivisionTemplateId(long divisionTemplateId)
        {
            var response = await scheduleTemplateRepository.GetByDivisionTemplateId(divisionTemplateId);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("getById/{scheduleTemplateId:long}")]
        [Authorize(Roles = "Directivo,Preceptor")]
        public async Task<ActionResult<ResponseDTO<ScheduleTemplateDTO>>> GetById(long scheduleTemplateId)
        {
            var response = await scheduleTemplateRepository.GetById(scheduleTemplateId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("{divisionTemplateId:long}")]
        [Authorize(Roles = "Directivo,Preceptor")]
        public async Task<ActionResult<ResponseDTO<string>>> Post(long divisionTemplateId, ScheduleTemplatePostDTO scheduleTemplateDTO)
        {
            var response = await scheduleTemplateRepository.Post(divisionTemplateId, scheduleTemplateDTO);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
