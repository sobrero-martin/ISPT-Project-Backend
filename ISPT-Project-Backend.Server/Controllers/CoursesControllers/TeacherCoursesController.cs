using BD.Entidades;
using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BD;
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
    private readonly AppDbContext db;

    public TeacherCoursesController(ITeacherCourses teacherCoursesRepository, IDivisionExamRepository divisionExamRepository, IAttendanceRepository attendanceRepository, IGradesRepository gradesRepository, AppDbContext db)
    {
        this.teacherCoursesRepository = teacherCoursesRepository;
        this.divisionExamRepository = divisionExamRepository;
        this.attendanceRepository = attendanceRepository;
        this.gradesRepository = gradesRepository;
        this.db = db;
    }

    [HttpPost("{divisionId:long}/close")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> CloseDivision(long divisionId)
    {
        if (!long.TryParse(User.FindFirst("PersonId")?.Value, out var teacherId)) return Unauthorized();
        return Respond(await teacherCoursesRepository.CloseDivision(divisionId, teacherId));
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
    public async Task<IActionResult> CreateEvaluation(DivisionExamPostDTO divisionExamDTO)
    {
        if (await IsClosed(divisionExamDTO.DivisionId)) return ClosedResponse();
        var res = await divisionExamRepository.CreateEvaluation(divisionExamDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpDelete("evaluations/{evaluationId:long}")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> DeleteEvaluation(long evaluationId)
    {
        var divisionId = await db.DivisionExams.Where(e => e.Id == evaluationId).Select(e => (long?)e.DivisionId).FirstOrDefaultAsync();
        if (divisionId.HasValue && await IsClosed(divisionId.Value)) return ClosedResponse();
        var res = await divisionExamRepository.DeleteEvaluation(evaluationId);
        return StatusCode((int)res.StatusCode, res);
    }

    // GRADES
    [HttpGet("evaluations/{evaluationId:long}/grades")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> GetEvaluationGrades(long evaluationId) => Respond(await gradesRepository.GetGrades(evaluationId));

    [HttpPut("evaluations/{evaluationId:long}/grades")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> SaveEvaluationGrades(long evaluationId, List<CourseGradeUpdateDTO> grades)
    {
        var divisionId = await db.DivisionExams.Where(e => e.Id == evaluationId).Select(e => (long?)e.DivisionId).FirstOrDefaultAsync();
        if (divisionId.HasValue && await IsClosed(divisionId.Value)) return ClosedResponse();
        return Respond(await gradesRepository.SaveGrades(evaluationId, grades));
    }

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
    public async Task<IActionResult> CreateAttendanceDay(long divisionId, [FromQuery] DateTime date)
    {
        if (await IsClosed(divisionId)) return ClosedResponse();
        return Respond(await attendanceRepository.CreateToday(divisionId, date));
    }

    [HttpGet("attendance/day/{dayId:long}")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> GetDailyAttendance(long dayId) => Respond(await attendanceRepository.GetDaily(dayId));

    [HttpPut("attendance/day/{dayId:long}")]
    [Authorize(Roles = "Docente")]
    public async Task<IActionResult> UpdateDailyAttendance(long dayId, List<AttendanceUpdateDTO> data)
    {
        var divisionId = await db.AttendanceDays.Where(d => d.Id == dayId).Select(d => (long?)d.DivisionId).FirstOrDefaultAsync();
        if (divisionId.HasValue && await IsClosed(divisionId.Value)) return ClosedResponse();
        return Respond(await attendanceRepository.UpdateDaily(dayId, data));
    }

    private Task<bool> IsClosed(long divisionId) => db.Divisions.AnyAsync(d => d.Id == divisionId && d.DivisionState == "Closed");
    private IActionResult ClosedResponse() => StatusCode(409, new ResponseDTO<string> { StatusCode = System.Net.HttpStatusCode.Conflict, Message = "El curso está cerrado y no admite cambios.", Object = null });

    private IActionResult Respond<T>(ResponseDTO<T> response) => StatusCode((int)response.StatusCode, response);
}
