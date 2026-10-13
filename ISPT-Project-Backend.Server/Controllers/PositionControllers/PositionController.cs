using DTO.DTOs.PersonDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Repository.Positions;

namespace ISPT_Project_Backend.Server.Controllers;

[ApiController]
[Route("api-v1/positions")]
public class PositionController : ControllerBase
{
    private readonly IPositionRepository positionRepository;

    public PositionController(IPositionRepository positionRepository)
    {
        this.positionRepository = positionRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> GetAllPersonal()
    {
        var res = await positionRepository.GetAllPersonal();
        return StatusCode((int)res.StatusCode, res);
    }
    
    
    [HttpGet("{id:long}")]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> GetPositionById(long id)
    {
        var res = await positionRepository.GetPositionById(id);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet("contact/{personId:long}")]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> GetContactByPersonId(long personId)
    {
        var res = await positionRepository.GetContactByPersonId(personId);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet("observation/{personId:long}")]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> GetObservationByPersonId(long personId)
    {
        var res = await positionRepository.GetObservationByPersonId(personId);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> CreatePosition(PersonDTO personDTO)
    {
        var res = await positionRepository.AddPerson(personDTO);
        return StatusCode((int)res.StatusCode, res);
    }
    
    [HttpPost("add-with-cuil")]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> AddPersonInPosition(PersonWithCUIL personWithCUIL)
    {
        var res = await positionRepository.AddRoleToPerson(personWithCUIL);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> UpdatePosition(PersonDTO personDTO)
    {
        var res = await positionRepository.EditPerson(personDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut("contact")]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> UpdateContact(ContactDTO contactDTO)
    {
        var res = await positionRepository.EditContact(contactDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut("observation")]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> UpdateObservation(ObservationDTO observationDTO)
    {
        var res = await positionRepository.EditObservation(observationDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeletePosition(long id)
    {
        var res = await positionRepository.RemovePersonWithPosition(id);
        return StatusCode((int)res.StatusCode, res);
    }
}