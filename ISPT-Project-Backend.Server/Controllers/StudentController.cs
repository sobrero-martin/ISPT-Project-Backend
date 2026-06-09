using DTO.DTOs.PersonDTO;
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
    public async Task<IActionResult> GetAllStudents()
    {
        var res = await studentRepository.GetAllPeopleByRol();
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent(PersonDTO personDTO)
    {
        var res = await studentRepository.AddPerson(personDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStudent(PersonDTO personDTO)
    {
        var res = await studentRepository.EditPerson(personDTO);
        return StatusCode((int)res.StatusCode, res);
    }
}