using DTO.DTOs.PersonDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Repository;

namespace ISPT_Project_Backend.Server.Controllers;

[ApiController]
[Route("api-v1/students")]
public class StudentController : ControllerBase
{
    private readonly IStudentRepository studentRepository;

    public StudentController(IStudentRepository studentRepository)
    {
        this.studentRepository = studentRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> GetAllStudents()
    {
        var res = await studentRepository.GetAllStudents();
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet("{id:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> GetStudentById(long id)
    {
        var res = await studentRepository.GetPerson(id);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet("contact/{personId:long}")]
    [Authorize(Roles = "Directivo,Preceptor,Preceptor_Auxiliar,Docente")]
    public async Task<IActionResult> GetContactByPersonId(long personId)
    {
        var res = await studentRepository.GetContactByPersonId(personId);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet("observation/{personId:long}")]
    [Authorize(Roles = "Directivo,Preceptor,Preceptor_Auxiliar,Docente")]
    public async Task<IActionResult> GetObservationByPersonId(long personId)
    {
        var res = await studentRepository.GetObservationByPersonId(personId);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> CreateStudent(PersonDTO personDTO)
    {
        var res = await studentRepository.AddPerson(personDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> UpdateStudent(PersonDTO personDTO)
    {
        var res = await studentRepository.EditPerson(personDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut("contact")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> UpdateContact(ContactDTO contactDTO)
    {
        var res = await studentRepository.EditContact(contactDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut("observation")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> UpdateObservation(ObservationDTO observationDTO)
    {
        var res = await studentRepository.EditObservation(observationDTO);
        return StatusCode((int)res.StatusCode, res);
    }

}