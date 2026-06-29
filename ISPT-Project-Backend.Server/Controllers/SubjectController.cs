using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Careers;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api-v1/subjects")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectRepository subjectRepository;

        public SubjectController(ISubjectRepository subjectRepository)
        {
            this.subjectRepository = subjectRepository;
        }


        [HttpGet("curriculum/{curriculumId:long}")]
        [Authorize(Roles = "Directivo,Preceptor")]
        public async Task<ActionResult<ResponseDTO<List<SubjectDTO>>>> GetByCurriculum(long curriculumId)
        {
            var response = await subjectRepository.GetByCurriculum(curriculumId);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:long}")]
        [Authorize(Roles = "Directivo,Preceptor")]
        public async Task<ActionResult<ResponseDTO<SubjectDTO>>> GetById(long id)
        {
            var response = await subjectRepository.GetById(id);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{curriculumId:long}/{subjectId:long}")]
        [Authorize(Roles = "Directivo,Preceptor")]
        public async Task<ActionResult<ResponseDTO<List<SubjectCorrelativesDTO>>>> GetPossibleCorrelatives(long curriculumId, long subjectId)
        {
            var response = await subjectRepository.GetPossibleCorrelatives(curriculumId, subjectId);

            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPost]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<SubjectDTO>>> Post(SubjectPostDTO subject)
        {
            var response = await subjectRepository.Post(subject);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:long}")]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<SubjectDTO>>> Put(long id, SubjectPostDTO subject)
        {
            var response = await subjectRepository.Put(id, subject);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("schoolyear/{schoolYearId:long}")]
        [Authorize(Roles = "Directivo,Preceptor")]
        public async Task<ActionResult<ResponseDTO<List<SubjectDTO>>>> GetBySchoolYear(long schoolYearId)
        {
            var response = await subjectRepository.GetBySchoolYear(schoolYearId);

            return StatusCode((int)response.StatusCode, response);
        }

    }
}