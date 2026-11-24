using BD.Entidades;
using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Courses;
using Repositorio.Repository;
using Repositorio.Implementations.Assistance;

namespace ISPT_Project_Backend.Server.Controllers.CoursesControllers;

[ApiController]
[Route("api-v1/teacher-courses")]
public class TeacherCoursesController :  ControllerBase
{
    private readonly ITeacherCourses teacherCoursesRepository;
    private readonly IDivisionExamRepository divisionExamRepository;
    private readonly IAttendanceRepository attendanceRepository;
    private readonly IGradesRepository gradesRepository;

    public TeacherCoursesController(ITeacherCourses teacherCoursesRepository, IDivisionExamRepository divisionExamRepository, IAttendanceRepository attendanceRepository, IGradesRepository gradesRepository)
    {
        this.teacherCoursesRepository = teacherCoursesRepository;
        this.divisionExamRepository = divisionExamRepository;
        this.attendanceRepository = attendanceRepository;
        this.gradesRepository = gradesRepository;
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
    [Authorize(Roles = "Docente")]
    public async Task<ActionResult<ResponseDTO<string>>> CreateEvaluation(DivisionExamPostDTO divisionExamDTO)
    {
        var res = await divisionExamRepository.CreateEvaluation(divisionExamDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpDelete("evaluations/{evaluationId:long}")]
    [Authorize(Roles = "Docente")]
    public async Task<ActionResult<ResponseDTO<string>>> DeleteEvaluation(long evaluationId)
    {
        var res = await divisionExamRepository.DeleteEvaluation(evaluationId);
        return StatusCode((int)res.StatusCode, res);
    }

    // GRADES
    [HttpGet("evaluations/{evaluationId:long}/grades")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> GetEvaluationGrades(long evaluationId) => Respond(await gradesRepository.GetGrades(evaluationId));

    [HttpPut("evaluations/{evaluationId:long}/grades")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> SaveEvaluationGrades(long evaluationId, List<CourseGradeUpdateDTO> grades) => Respond(await gradesRepository.SaveGrades(evaluationId, grades));

    // SCHEDULE
    [HttpGet("schedules/{divisionId:long}")]
    [Authorize(Roles = "Docente")]
    public async Task<ActionResult<ResponseDTO<ScheduleDTO>>> GetSchedulesByDivisionId(long divisionId)
    {
        var res = await teacherCoursesRepository.GetScheduleByDivisionId(divisionId);
        return StatusCode((int)res.StatusCode, res);
    }

    // ATTENDANCE
    [HttpGet("attendance/{divisionId:long}")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> GetAttendanceDays(long divisionId) => Respond(await attendanceRepository.GetDays(divisionId));

    [HttpPost("attendance/{divisionId:long}/today")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> CreateAttendanceDay(long divisionId, [FromQuery] DateTime date) => Respond(await attendanceRepository.CreateToday(divisionId, date));

    [HttpGet("attendance/day/{dayId:long}")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> GetDailyAttendance(long dayId) => Respond(await attendanceRepository.GetDaily(dayId));

    [HttpPut("attendance/day/{dayId:long}")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> UpdateDailyAttendance(long dayId, List<AttendanceUpdateDTO> data) => Respond(await attendanceRepository.UpdateDaily(dayId, data));

    private IActionResult Respond<T>(ResponseDTO<T> response) => StatusCode((int)response.StatusCode, response);
}
