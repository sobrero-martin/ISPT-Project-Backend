using DTO.DTOs.PersonDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Repository;

namespace ISPT_Project_Backend.Server.Controllers;

[ApiController]
[Route("api-v1/teachers")]
public class TeacherController : ControllerBase
{
    private readonly ITeacherRepository teacherRepository;

    public TeacherController(ITeacherRepository teacherRepository)
    {
        this.teacherRepository = teacherRepository;
    }
    
    [HttpGet]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> GetAllTeachers()
    {
        var res = await teacherRepository.GetAllTeachers();
        return StatusCode((int)res.StatusCode, res);
    }
    
    [HttpGet("{id:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> GetTeacherById(long id)
    {
        var res = await teacherRepository.GetPerson(id);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet("contact/{personId:long}")]
    [Authorize(Roles = "Directivo,Preceptor,Preceptor_Auxiliar,Docente")]
    public async Task<IActionResult> GetContactByPersonId(long personId)
    {
        var res = await teacherRepository.GetContactByPersonId(personId);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet("observation/{personId:long}")]
    [Authorize(Roles = "Directivo,Preceptor,Preceptor_Auxiliar,Docente")]
    public async Task<IActionResult> GetObservationByPersonId(long personId)
    {
        var res = await teacherRepository.GetObservationByPersonId(personId);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> CreateTeacher(PersonDTO personDTO)
    {
        var res = await teacherRepository.AddPerson(personDTO);
        return StatusCode((int)res.StatusCode, res);
    }
    
    [HttpPost("add-with-cuil")]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> AddPersonInTeacher(PersonWithCUIL personWithCUIL)
    {
        personWithCUIL.RoleName = string.Empty;
        var res = await teacherRepository.AddRoleToPerson(personWithCUIL);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> UpdateTeacher(PersonDTO personDTO)
    {
        var res = await teacherRepository.EditPerson(personDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut("contact")]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> UpdateContact(ContactDTO contactDTO)
    {
        var res = await teacherRepository.EditContact(contactDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut("observation")]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> UpdateObservation(ObservationDTO observationDTO)
    {
        var res = await teacherRepository.EditObservation(observationDTO);
        return StatusCode((int)res.StatusCode, res);
    }
}