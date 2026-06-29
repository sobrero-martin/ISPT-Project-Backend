using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.SchoolYears;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api-v1/schoolyears")]
    public class SchoolYearController : ControllerBase
    {
        private readonly ISchoolYearRepository schoolYearRepository;

        public SchoolYearController(ISchoolYearRepository schoolYearRepository)
        {
            this.schoolYearRepository = schoolYearRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDTO<List<SchoolYearDTO>>>> GetFull()
        {
            var response = await schoolYearRepository.GetFull();
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("raw")]
        public async Task<ActionResult<ResponseDTO<List<SchoolYearPostDTO>>>> GetRaw()
        {
            var response = await schoolYearRepository.GetRaw();
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ResponseDTO<SchoolYearPostDTO>>> GetById(long id)
        {
            var response = await schoolYearRepository.GetById(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDTO<SchoolYearPostDTO>>> Post(SchoolYearPostDTO schoolYear)
        {
            var response = await schoolYearRepository.Post(schoolYear);

            return StatusCode((int)response.StatusCode, response);
        }

    }
}
