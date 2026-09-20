using DTO.DTOs.DTO_Response;
using DTO.DTOs.ExamDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Careers;
using Repositorio.Implementations.Exams;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api-v1/final-exams")]
    public class FinalExamController : ControllerBase
    {
        private readonly IFinalExamRepository finalExamRepository;

        public FinalExamController(IFinalExamRepository finalExamRepository)
        {
            this.finalExamRepository = finalExamRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Directivo,Preceptor,Docente")]
        public async Task<ActionResult<ResponseDTO<List<FinalExamDTO>>>> GetFull()
        {
            var response = await finalExamRepository.GetFull();
            return StatusCode((int)response.StatusCode, response);
        }
        
        [HttpGet("{id:long}")]
        [Authorize(Roles = "Directivo,Preceptor,Docente")]
        public async Task<ActionResult<ResponseDTO<List<FinalExamDTO>>>> GetFinalExamById(long id)
        {
            var response = await finalExamRepository.GetFinalExamById(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<string>>> Post(FinalExamPostDTO exam)
        {
            var response = await finalExamRepository.Post(exam);
            return StatusCode((int)response.StatusCode, response);
        }
        
        [HttpPut]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<string>>> Put(FinalExamPostDTO exam)
        {
            var response = await finalExamRepository.Edit(exam);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<string>>> Delete(long id)
        {
            var response = await finalExamRepository.Delete(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
