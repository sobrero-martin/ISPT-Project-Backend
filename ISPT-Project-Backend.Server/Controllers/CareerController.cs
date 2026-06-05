using BD.Entidades;
using DTO.DTOs;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Repository;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api/careers")]
    public class CareerController : ControllerBase
    {
        private readonly ICareerRepository careerRepository;

        public CareerController(ICareerRepository careerRepository)
        {
            this.careerRepository = careerRepository;
        }


        [HttpGet]
        public async Task<ActionResult<List<CareerDTO>>> GetFull()
        {
            var users = await careerRepository.GetFull();

            if (users == null)
            {
                return NotFound("No se encontraron carreras");
            }

            if (users.Count == 0)
            {
                return NotFound("No existen carreras");
            }

            return Ok(users);
        }


        [HttpPost]
        public async Task<ActionResult<int>> Post(Career career)
        {
            try
            {
                await careerRepository.Post(career);
                return Ok(career.Id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult> Put(long id, Career career)
        {
            var result = await careerRepository.Put(id, career);
            return Ok($"Career with id {id} correctly updated");
        }

    }
}