using DTO.DTOs.SchoolYearDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.SchoolYears;
using Repositorio.Repository;

namespace ISPT_Project_Backend.Server.Controllers
{
    [ApiController]
    [Route("api-v1/file-divisions")]
    public class FileDivisionController : ControllerBase
    {
        private readonly IFileDivisionRepository fileDivisionRepository;

        public FileDivisionController(IFileDivisionRepository fileDivisionRepository)
        {
            this.fileDivisionRepository = fileDivisionRepository;
        }

        [HttpGet("{divisionId:long}")]
        public async Task<IActionResult> GetFileDivisions(long divisionId)
        {
            var response = await fileDivisionRepository.GetByDivisionId(divisionId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> PostFileDivision(StudentFileDivisionPostDTO studentFileDivisionDTO)
        {
            var response = await fileDivisionRepository.Post(studentFileDivisionDTO);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("school-year/{schoolYearId:long}/inscription-requests")]
        [Authorize(Roles = "Preceptor")]
        public async Task<ActionResult<ResponseDTO<List<InscriptionRequestDTO>>>> GetInscriptionRequests(long schoolYearId)
        {
            var response = await fileDivisionRepository.GetInscriptionRequests(schoolYearId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("school-year/{schoolYearId:long}/inscription-requests")]
        [Authorize(Roles = "Preceptor")]
        public async Task<ActionResult<ResponseDTO<string>>> ReviewInscriptionRequests(long schoolYearId, [FromBody] ReviewInscriptionRequestsDTO review)
        {
            var response = await fileDivisionRepository.ReviewInscriptionRequests(schoolYearId, review);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
