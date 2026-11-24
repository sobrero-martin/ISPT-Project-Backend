using DTO.DTOs.DTO_Response;
using DTO.DTOs.ExamDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Exams;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api-v1/iefis")]
    public class IEFIController : ControllerBase
    {
        private readonly IIEFIRepository iefiRepository;

        public IEFIController(IIEFIRepository iefiRepository)
        {
            this.iefiRepository = iefiRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Directivo,Preceptor,Docente")]
        public async Task<ActionResult<ResponseDTO<List<IEFIDTO>>>> GetFull()
        {
            var response = await iefiRepository.GetFull();
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:long}")]
        [Authorize(Roles = "Directivo,Preceptor,Docente")]
        public async Task<ActionResult<ResponseDTO<IEFIPostDTO>>> GetIEFIById(long id)
        {
            var response = await iefiRepository.GetIEFIById(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<string>>> Post(IEFIPostDTO iefi)
        {
            var response = await iefiRepository.Post(iefi);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<string>>> Put(IEFIPostDTO iefi)
        {
            var response = await iefiRepository.Edit(iefi);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<string>>> Delete(long id)
        {
            var response = await iefiRepository.Delete(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("division/{divisionId:long}/students")]
        [Authorize(Roles = "Preceptor,Docente,Estudiante")]
        public async Task<ActionResult<ResponseDTO<List<IEFIDivisionStudentDTO>>>> GetStudents(long divisionId)
        {
            if (User.IsInRole("Estudiante"))
            {
                var personId = User.FindFirst("PersonId")?.Value;
                if (!long.TryParse(personId, out var parsed)) return Unauthorized();
                var studentResponse = await iefiRepository.GetStudents(divisionId, parsed);
                studentResponse.Object = studentResponse.Object?.Where(x => x.Registered).ToList();
                return StatusCode((int)studentResponse.StatusCode, studentResponse);
            }
            var response = await iefiRepository.GetStudents(divisionId, registerEligible: User.IsInRole("Docente"));
            if (User.IsInRole("Preceptor")) response.Object = response.Object?.Where(x => x.Registered).ToList();
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("division/{divisionId:long}/students/{fileDivisionId:long}")]
        [Authorize(Roles = "Docente")]
        public async Task<ActionResult<ResponseDTO<string>>> RegisterStudent(long divisionId, long fileDivisionId)
        {
            var response = await iefiRepository.RegisterStudent(divisionId, fileDivisionId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("division/{divisionId:long}/students/{fileDivisionId:long}/grade")]
        [Authorize(Roles = "Docente")]
        public async Task<ActionResult<ResponseDTO<string>>> SaveGrade(long divisionId, long fileDivisionId, [FromBody] IEFIGradeDTO dto)
        {
            var response = await iefiRepository.SaveGrade(divisionId, fileDivisionId, dto.Grade);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
