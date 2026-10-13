using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Careers;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api-v1/divisions")]
    public class DivisionController : ControllerBase
    {
        private readonly IDivisionRepository divisionRepository;

        public DivisionController(IDivisionRepository divisionRepository)
        {
            this.divisionRepository = divisionRepository;
        }

        [HttpGet("school-year/{schoolYearId:long}/subject/{subjectId:long}")]
        public async Task<IActionResult> GetBySchoolYearSubject(long schoolYearId, long subjectId)
        {
            var response = await divisionRepository.GetBySchoolYearSubject(schoolYearId, subjectId);
            return Ok(response);
        }
    }
}
