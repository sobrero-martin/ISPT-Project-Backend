using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations;

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
        public async Task<ActionResult<ResponseDTO<List<SubjectDTO>>>> GetByCurriculum(long curriculumId)
        {
            var response = await subjectRepository.GetByCurriculum(curriculumId);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ResponseDTO<SubjectDTO>>> GetById(long id)
        {
            var response = await subjectRepository.GetById(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{curriculumId:long}/{subjectId:long}")]
        public async Task<ActionResult<ResponseDTO<List<SubjectDTO>>>> GetPossibleCorrelatives(long curriculumId, long subjectId)
        {
            var response = await subjectRepository.GetPossibleCorrelatives(curriculumId, subjectId);

            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPost]
        public async Task<ActionResult<ResponseDTO<SubjectDTO>>> Post(Subject subject)
        {
            var response = await subjectRepository.Post(subject);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<ResponseDTO<SubjectDTO>>> Put(long id, Subject subject)
        {
            var response = await subjectRepository.Put(id, subject);
            return StatusCode((int)response.StatusCode, response);
        }

    }
}