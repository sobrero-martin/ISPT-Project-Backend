using BD;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.CoursesDTO;
using DTO.DTOs.CoursesDTO.InscriptionForm;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Students;
using System.Net;

namespace Repositorio.Repository;

public class StudentCourses : IStudentCourses
{

    private readonly AppDbContext bbdd;

    public StudentCourses(AppDbContext bbdd)
    {
        this.bbdd = bbdd;
    }

    public async Task<ResponseDTO<List<CareerInscriptionDTO>>> GetAllAvailableCareerInscription()
    {
        try
        {
            var careers = await bbdd.SchoolYears.Where(sy => sy.StartDate.Date <= DateTime.Today && DateTime.Today <= sy.EndDate.Date)
                .Select(sy => new
                {
                    sy.CurriculumId,
                    sy.Curriculum.Career.Name,
                    sy.Curriculum.Duration,
                    sy.Curriculum.Career.Title,
                    sy.SchoolYearNumber,
                    sy.EndDate
                }).Distinct().ToListAsync();
            if (careers.Count == 0)
                return new ResponseDTO<List<CareerInscriptionDTO>>
                {
                    Message = "¡No hay fechas de inscripción disponibles por el momento!",
                    Object = null,
                    StatusCode = HttpStatusCode.Locked
                };

            return new ResponseDTO<List<CareerInscriptionDTO>>
            {
                Message = "¡Operación éxitosa!",
                Object = careers.Select(c => new CareerInscriptionDTO
                {
                    CurriculumId = c.CurriculumId,
                    CareerName = c.Name,
                    Duration = c.Duration,
                    Title = c.Title,
                    VigencyDate = c.EndDate.ToString("dd/MM/yyyy"),
                    SchoolYearNumber = c.SchoolYearNumber
                }).ToList(),
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al intentar obtener las carreras para inscripción: " + e.Message);

            return new ResponseDTO<List<CareerInscriptionDTO>>()
            {
                Message = "¡Error al intentar obtener las carreras para inscribirse! Contacte con preceptoría.",
                Object = null,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }
    public async Task<ResponseDTO<InscriptionFormDTO>> GetAllSubjectsFromCareer(long curriculumId, int schoolYear, long studentId)
    {
        try
        {
            var divisionesPasadas = await bbdd.FileDivisions.Where(fd => bbdd.FileCurriculum
                .Any(fc => fc.Id == fd.FileCurriculumId && fc.CurriculumId == curriculumId &&
                bbdd.Files.Any(f => f.Id == fc.FileId && f.StudentId == studentId))).ToListAsync();

            var cicloActuales = await bbdd.SchoolYears
                .Where(sy => sy.CurriculumId == curriculumId && sy.SchoolYearNumber == schoolYear).ToListAsync();
            var divisionesActuales = await bbdd.Divisions.Where(d => cicloActuales.Select(c => c.Id).Contains(d.SchoolYearId))
                .Include(d => d.DivisionTemplate).ToListAsync();

            var espaciosCurriculares = await bbdd.Subjects.Where(s => s.CurriculumId == curriculumId && 
            cicloActuales.Select(c => c.YearNumber).Contains(s.Year)).ToListAsync();

            if (divisionesPasadas.Count != 0)
            {
                var correlativas = await bbdd.Correlatives.Where(c => espaciosCurriculares.Select(e => e.Id).Contains(c.Id)).ToListAsync();
                throw new NotImplementedException();
            }
            else // Todavía no comenzo a cursar nada
            {
                return new ResponseDTO<InscriptionFormDTO>()
                {
                    Message = "",
                    Object = new InscriptionFormDTO
                    {
                        YearsAvailable = cicloActuales.Count,
                        Subjects = espaciosCurriculares.Select(s => new SubjectsInscriptionDTO
                        {
                            IsAvailable = s.Year == 1 ? DTO.ENUM.SubjectStudentStatus.Available : DTO.ENUM.SubjectStudentStatus.Unavailable,
                            SubjectName = s.Name,
                            YearAvailable = s.Year,
                            Divisions = divisionesActuales.Where(d => d.DivisionTemplate!.SubjectId == s.Id)
                            .Select(d => new DivisionInscriptionDTO
                            {
                                DivisionId = d.Id,
                                DivisionName = d.DivisionTemplate!.Name
                            }).ToList()
                        }).ToList(),
                    },
                    StatusCode = HttpStatusCode.OK
                };
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al intentar obtener los espacios curriculares para inscripción: " + e.Message);

            return new ResponseDTO<InscriptionFormDTO>()
            {
                Message = "¡Error al cargar los espacios curriculares para inscribirse! Contacte con preceptoría.",
                Object = null,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseDTO<List<ScheduleInscriptionDTO>>> GetAllDivisionSchedules(List<long> divisionIds)
    {
        try
        {
            return new ResponseDTO<List<ScheduleInscriptionDTO>>()
            {
                Message = "¡Operación éxitosa!",
                Object = await bbdd.Schedules.Where(s => divisionIds.Contains(s.DivisionId)).Select(s => new ScheduleInscriptionDTO
                {
                    Division = s.Division.DivisionTemplate.Name,
                    Day = s.Day,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToListAsync(),
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception e) 
        {
            Console.WriteLine($"Error al intentar obtener los horarios del espacios curriculares para inscripción: " + e.Message);

            return new ResponseDTO<List<ScheduleInscriptionDTO>>()
            {
                Message = "¡Error al cargar los horarios del espacios curricular! Contacte con preceptoría.",
                Object = null,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseDTO<string>> Inscription()
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al intentar inscribir al usuario: " + e.Message);

            return new ResponseDTO<string>()
            {
                Message = "¡Error al realizar la inscripción, intenta de nuevo! Si el error persiste ponte en contacto con preceptoría.",
                Object = null,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }





    public Task GetCoursesByStudentId(long studentId)
    {
        throw new NotImplementedException();
    }
}