using DTO.DTOs.TeachersDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Repository;

namespace ISPT_Project_Backend.Server.Controllers;

[ApiController]
[Route("api-v1/teachers-division")]
public class TeacherDivisionController : ControllerBase
{
    private readonly ITeacherDivisionRepository teacherDivisionRepository;

    public TeacherDivisionController(ITeacherDivisionRepository teacherDivisionRepository)
    {
        this.teacherDivisionRepository = teacherDivisionRepository;
    }
    
    [HttpGet("division/{id:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> GetAllTeachersByDivisionId(long id)
    {
        var res = await teacherDivisionRepository.GetAllTeachersByDivisionId(id);
        return StatusCode((int)res.StatusCode, res);
    }
    
    [HttpGet("{id:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> GetTeacherDivision(long id)
    {
        var res = await teacherDivisionRepository.GetTeacherDivision(id);
        return StatusCode((int)res.StatusCode, res);
    }
    
    [HttpGet("observation/{teacherDivisionID:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<IActionResult> GetObservationByTeacherDivisionId(long teacherDivisionId)
    {
        var res = await teacherDivisionRepository.GetObservationByTeacherDivisionId(teacherDivisionId);
        return StatusCode((int)res.StatusCode, res);
    }
    
    [HttpPost]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> AssignTeacher(TeacherDivisionPostDTO teacherDivisionPostDTO)
    {
        var res = await teacherDivisionRepository.AsignTeacherIntoDivision(teacherDivisionPostDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> UpdateAssignation(TeacherDivisionPostDTO teacherDivisionPutDTO)
    {
        var res = await teacherDivisionRepository.UpdateAsignationInDivision(teacherDivisionPutDTO);
        return StatusCode((int)res.StatusCode, res);
    }
    
    [HttpPut("observation")]
    [Authorize(Roles = "Directivo")]
    public async Task<IActionResult> UpdateObservation(TeacherDivisionObservationDTO observationDTO)
    {
        var res = await teacherDivisionRepository.EditObservation(observationDTO);
        return StatusCode((int)res.StatusCode, res);
    }
    
}