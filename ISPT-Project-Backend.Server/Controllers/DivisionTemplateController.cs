using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Careers;
using Repositorio.Repository.Careers;


namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api-v1/division-templates")]
    public class DivisionTemplateController : ControllerBase
    {
        private readonly IDivisionTemplateRepository divisionTemplateRepository;

        public DivisionTemplateController(IDivisionTemplateRepository divisionTemplateRepository)
        {
            this.divisionTemplateRepository = divisionTemplateRepository;
        }



        [HttpGet("subject/{subjectId:long}")]
        public async Task<ActionResult<ResponseDTO<List<DivisionTemplateDTO>>>> GetBySubject(long subjectId)
        {
            var response = await divisionTemplateRepository.GetBySubject(subjectId);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("subject/{subjectId:long}")]
        public async Task<ActionResult<ResponseDTO<DivisionTemplateDTO>>> Post(long subjectId)
        {
            var response = await divisionTemplateRepository.Post(subjectId);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
