using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Careers;
using Repositorio.Repository;

namespace ISPT_Project_Backend.Server.Controllers
{

    [ApiController]
    [Route("api-v1/correlatives")]
    public class CorrelativeController : ControllerBase
    {
        private readonly ICorrelativeRepository correlativeRepository;

        public CorrelativeController(ICorrelativeRepository correlativeRepository)
        {
            this.correlativeRepository = correlativeRepository;
        }



        [HttpPost("{subjectId:long}/saveChanges")]
        [Authorize(Roles = "Directivo")]
        public async Task<ActionResult<ResponseDTO<bool>>> SaveChanges(long subjectId, [FromBody] List<CorrelativeChangeDTO> changes)
        {
            var response = await correlativeRepository.SaveChanges(subjectId, changes);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
