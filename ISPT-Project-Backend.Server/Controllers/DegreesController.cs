using DTO.DTOs.StudentsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Repository;

namespace ISPT_Project_Backend.Server.Controllers;

[ApiController]
[Route("api-v1/degrees")]
public class DegreesController : ControllerBase
{
    private readonly IDegreesRepository degreesRepository;

    public DegreesController(IDegreesRepository degreesRepository)
    {
        this.degreesRepository = degreesRepository;
    }

    [HttpGet("{personId:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> GetDegreesByPersonId(long personId)
    {
        var res = await degreesRepository.GetDegreesByPersonId(personId);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> createDegree(DegreeDTO degreeDTO)
    {
        var res = await degreesRepository.createDegree(degreeDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpDelete("{degreeId:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> deleteDegree(long degreeId)
    {
        var res = await degreesRepository.deleteDegree(degreeId);
        return StatusCode((int)res.StatusCode, res);
    }
}