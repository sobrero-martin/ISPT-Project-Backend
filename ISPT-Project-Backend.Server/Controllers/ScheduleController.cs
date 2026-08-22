using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Careers;
using Repositorio.Implementations.SchoolYears;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api-v1/schedules")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleRepository scheduleRepository;

        public ScheduleController(IScheduleRepository scheduleRepository)
        {
            this.scheduleRepository = scheduleRepository;
        }

        [HttpGet("{divisionId:long}")]
        public async Task<IActionResult> GetByDivisionId(long divisionId)
        {
            var response = await scheduleRepository.GetByDivisionId(divisionId);
            return Ok(response);
        }
    }
}
