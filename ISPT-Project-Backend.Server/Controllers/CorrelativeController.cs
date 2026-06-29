using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
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



        [HttpPost]
        public async Task<ActionResult<ResponseDTO<CorrelativeDTO>>> Post(Correlative correlative)
        {
            var response = await correlativeRepository.Post(correlative);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{subjectId:long}/{correlativeId:long}")]
        public async Task<ActionResult<ResponseDTO<bool>>> Delete(long subjectId, long correlativeId)
        {
            var response = await correlativeRepository.Delete(subjectId, correlativeId);

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("{subjectId:long}/saveChanges")]
        public async Task<ActionResult<ResponseDTO<bool>>> SaveChanges(long subjectId, [FromBody] List<CorrelativeChangeDTO> changes)
        {
            var response = await correlativeRepository.SaveChanges(subjectId, changes);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}
