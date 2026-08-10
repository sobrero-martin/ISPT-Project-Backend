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

        [HttpPost]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<string>>> Post(FinalExamPostDTO exam)
        {
            var response = await finalExamRepository.Post(exam);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
