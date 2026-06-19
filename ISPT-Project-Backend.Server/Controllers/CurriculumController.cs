using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api/curriculum")]
    public class CurriculumController : ControllerBase
    {
        private readonly ICurriculumRepository curriculumRepository;

        public CurriculumController(ICurriculumRepository curriculumRepository)
        {
            this.curriculumRepository = curriculumRepository;
        }


        [HttpGet("career/{careerId:long}")]
        public async Task<ActionResult<ResponseDTO<List<CurriculumDTO>>>> GetByCareer(long careerId)
        {
            var response = await curriculumRepository.GetByCareer(careerId);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ResponseDTO<CurriculumDTO>>> GetById(long id)
        {
            var response = await curriculumRepository.GetById(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDTO<CurriculumDTO>>> Post(Curriculum curriculum)
        {
                var response = await curriculumRepository.Post(curriculum);

                return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<ResponseDTO<string>>> Put(long id, Curriculum curriculum)
        {
            var response = await curriculumRepository.Put(id, curriculum);
            return StatusCode((int)response.StatusCode, response);
        }

    }
}