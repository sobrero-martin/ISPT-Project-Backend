using BD.Entidades;
using DTO.DTOs;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Repository;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectRepository subjectRepository;

        public SubjectController(ISubjectRepository subjectRepository)
        {
            this.subjectRepository = subjectRepository;
        }


        [HttpGet("curriculum/{curriculumId:long}")]
        public async Task<ActionResult<List<SubjectDTO>>> GetByCurriculum(long curriculumId)
        {
            var subjects = await subjectRepository.GetByCurriculum(curriculumId);

            if (subjects == null)
            {
                return NotFound("No se encontraron materias");
            }

            if (subjects.Count == 0)
            {
                return NotFound("No existen materias en este plan de estudio");
            }

            return Ok(subjects);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<SubjectDTO>> GetById(long id)
        {
            var subject = await subjectRepository.GetById(id);

            if (subject == null)
            {
                return NotFound("Materia no encontrada");
            }

            return Ok(subject);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Subject subject)
        {
            try
            {
                await subjectRepository.Post(subject);
                return Ok(subject.Id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult> Put(long id, Subject subject)
        {
            var result = await subjectRepository.Put(id, subject);
            return Ok($"Subject with id {id} correctly updated");
        }

    }
}