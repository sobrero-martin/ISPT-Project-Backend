using DTO.DTOs.CoursesDTO;
using DTO.DTOs.CoursesDTO.InscriptionForm;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.Students;
using System.Security.Claims;

namespace ISPT_Project_Backend.Server.Controllers.CoursesControllers
{
    [Route("api-v1/student-courses")]
    [ApiController]
    public class StudentCoursesController : ControllerBase
    {
        private readonly IStudentCourses studentCourses;

        public StudentCoursesController(IStudentCourses studentCourses)
        {
            this.studentCourses = studentCourses;
        }

        // INSCRIPTIONS
        [HttpGet("get-careers")]
        [Authorize(Roles = "Estudiante")]
        public async Task<ActionResult<ResponseDTO<List<CareerInscriptionDTO>>>> GetCareersForInscription()
        {
            var personId = User.FindFirst("PersonId")?.Value;
            if (!long.TryParse(personId, out var studentId)) return Unauthorized();

            var res = await studentCourses.GetAllAvailableCareerInscription(studentId);
            return StatusCode((int)res.StatusCode, res);
        }

        [HttpGet("{curriculumId:long}/get-subjects/{schoolYear:int}")]
        [Authorize(Roles = "Estudiante")]
        public async Task<ActionResult<ResponseDTO<List<InscriptionFormDTO>>>> GetAllSubjectsFromCareer(long curriculumId, int schoolYear)
        {
            var personId = User.FindFirst("PersonId")?.Value;
            if (!long.TryParse(personId, out var studentId)) return Unauthorized();

            var res = await studentCourses.GetAllSubjectsFromCareer(curriculumId, schoolYear, studentId);
            return StatusCode((int)res.StatusCode, res);
        }

        [HttpPost("get-subjects/schedules")]
        [Authorize(Roles = "Estudiante")]
        public async Task<ActionResult<ResponseDTO<List<ScheduleInscriptionDTO>>>> GetAllDivisionSchedules(List<long> divisionIds)
        {
                var res = await studentCourses.GetAllDivisionSchedules(divisionIds);
                return StatusCode((int)res.StatusCode, res);
        }

        [HttpPost("inscriptions")]
        [Authorize(Roles = "Estudiante")]
        public async Task<ActionResult<ResponseDTO<string>>> SendInscription([FromBody] StudentInscriptionDTO inscription)
        {
            var personId = User.FindFirst("PersonId")?.Value;
            if (!long.TryParse(personId, out var studentId)) return Unauthorized();
            if (inscription == null) return BadRequest();

            var response = await studentCourses.Inscription(inscription, studentId);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
