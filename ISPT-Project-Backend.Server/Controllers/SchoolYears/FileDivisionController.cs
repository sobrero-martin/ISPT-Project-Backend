using DTO.DTOs.SchoolYearDTO;
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
    }
}
