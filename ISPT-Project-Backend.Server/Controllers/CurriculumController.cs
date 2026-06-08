using BD.Entidades;
using DTO.DTOs;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Repository;

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
        public async Task<ActionResult<List<CurriculumDTO>>> GetByCareer(long careerId)
        {
            var users = await curriculumRepository.GetByCareer(careerId);

            if (users == null)
            {
                return NotFound("No se encontraron planes de estudio");
            }

            if (users.Count == 0)
            {
                return NotFound("No existen planes de estudio");
            }

            return Ok(users);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<CurriculumDTO>> GetById(long id)
        {
            var curriculum = await curriculumRepository.GetById(id);

            if (curriculum == null)
            {
                return NotFound("Plan de estudio no encontrado");
            }

            return Ok(curriculum);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Curriculum curriculum)
        {
            try
            {
                await curriculumRepository.Post(curriculum);
                return Ok(curriculum.Id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult> Put(long id, Curriculum curriculum)
        {
            var result = await curriculumRepository.Put(id, curriculum);
            return Ok($"Curriculum with id {id} correctly updated");
        }

    }
}