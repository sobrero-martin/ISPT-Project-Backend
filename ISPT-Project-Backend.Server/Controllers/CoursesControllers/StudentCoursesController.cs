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
            var res = await studentCourses.GetAllAvailableCareerInscription();
            return StatusCode((int)res.StatusCode, res);
        }

        [HttpGet("{curriculumId:long}/get-subjects/{schoolYear:int}")]
        [Authorize(Roles = "Estudiante")]
        public async Task<ActionResult<ResponseDTO<List<InscriptionFormDTO>>>> GetAllSubjectsFromCareer(long curriculumId, int schoolYear)
        {
            try
            {
                var res = await studentCourses.GetAllSubjectsFromCareer(
                curriculumId,
                schoolYear,
                long.Parse(User.FindFirst("PersonId")!.Value));
                return StatusCode((int)res.StatusCode, res);
            }
            catch (Exception e)
            {
                Console.WriteLine("Token sin PersonId");
                return StatusCode(500);
            }
        }

        [HttpPost("get-subjects/schedules")]
        [Authorize(Roles = "Estudiante")]
        public async Task<ActionResult<ResponseDTO<List<ScheduleInscriptionDTO>>>> GetAllDivisionSchedules(List<long> divisionIds)
        {
                var res = await studentCourses.GetAllDivisionSchedules(divisionIds);
                return StatusCode((int)res.StatusCode, res);
        }
    }
}
