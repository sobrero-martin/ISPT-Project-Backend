using BD.Entidades;
using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Courses;
using Repositorio.Repository;

namespace ISPT_Project_Backend.Server.Controllers.CoursesControllers;

[ApiController]
[Route("api-v1/teacher-courses")]
public class TeacherCoursesController :  ControllerBase
{
    private readonly ITeacherCourses teacherCoursesRepository;
    private readonly IDivisionExamRepository divisionExamRepository;

    public TeacherCoursesController(ITeacherCourses teacherCoursesRepository, IDivisionExamRepository divisionExamRepository)
    {
        this.teacherCoursesRepository = teacherCoursesRepository;
        this.divisionExamRepository = divisionExamRepository;
    }
    
    // COURSES
    [HttpGet("{teacherId:long}")]
    [Authorize(Roles = "Docente")]
    public async Task<ActionResult<ResponseDTO<List<TeacherCoursesDTO>>>> GetCoursesByTeacherId(long teacherId)
    {
        var res = await teacherCoursesRepository.GetCoursesByTeacherId(teacherId);
        return StatusCode((int)res.StatusCode, res);
    }

    // EVALUATIONS
    [HttpGet("evaluations/{divisionId:long}")]
    [Authorize(Roles = "Docente")]
    public async Task<ActionResult<ResponseDTO<List<DivisionExamReadDTO>>>> GetAllEvaluationsFromDivisionId(long divisionId)
    {
        var res = await divisionExamRepository.GetAllEvaluations(divisionId);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost("evaluations")]
    public async Task<ActionResult<ResponseDTO<string>>> CreateEvaluation(DivisionExamPostDTO divisionExamDTO)
    {
        var res = await divisionExamRepository.CreateEvaluation(divisionExamDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpDelete("evaluations/{evaluationId:long}")]
    public async Task<ActionResult<ResponseDTO<string>>> DeleteEvaluation(long evaluationId)
    {
        var res = await divisionExamRepository.DeleteEvaluation(evaluationId);
        return StatusCode((int)res.StatusCode, res);
    }

    // SCHEDULE
    [HttpGet("schedules/{divisionId:long}")]
    public async Task<ActionResult<ResponseDTO<ScheduleDTO>>> GetSchedulesByDivisionId(long divisionId)
    {
        var res = await teacherCoursesRepository.GetScheduleByDivisionId(divisionId);
        return StatusCode((int)res.StatusCode, res);
    }

    // ATTENDANCE
}