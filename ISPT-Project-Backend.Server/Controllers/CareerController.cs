using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Careers;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api-v1/careers")]
    public class CareerController : ControllerBase
    {
        private readonly ICareerRepository careerRepository;

        public CareerController(ICareerRepository careerRepository)
        {
            this.careerRepository = careerRepository;
        }


        [HttpGet]
        [Authorize(Roles = "Directivo,Preceptor")]
        public async Task<ActionResult<ResponseDTO<List<CareerDTO>>>> GetFull()
        {
            var response = await careerRepository.GetFull();
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:long}")]
        [Authorize(Roles = "Directivo,Preceptor")]
        public async Task<ActionResult<ResponseDTO<CareerDTO>>> GetById(long id)
        {
            var response = await careerRepository.GetById(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<int>>> Post(Career career)
        {
            var response = await careerRepository.Post(career);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:long}")]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<string>>> Put(long id, Career career)
        {
            var response = await careerRepository.Put(id, career);
            return StatusCode((int)response.StatusCode, response);
        }

    }
}