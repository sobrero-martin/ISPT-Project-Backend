using BD;
using BD.Entidades;
using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Repositorio.Repository;

public class TeacherCourses : ITeacherCourses
{
    private readonly AppDbContext bbdd;

    public TeacherCourses(AppDbContext bbdd)
    {
        this.bbdd = bbdd;
    }

    public async Task<ResponseDTO<List<TeacherCoursesDTO>>> GetCoursesByTeacherId(long teacherId)
    {
        try
        {
            var courses = await bbdd.TeacherDivisions
                .AsNoTracking()
                .Where(td => td.TeacherId == teacherId)
                .Select(td => new TeacherCoursesDTO
                {
                    DivisionId = td.DivisionId,
                    Division = td.Division.Name,
                    Subject = td.Division.Subject!.Name,
                    SchoolYear = td.Division.Subject.Year,
                    Career = td.Division.Subject.Curriculum!.Career!.Name
                })
                .ToListAsync();

            if (courses.Count == 0) throw new Exception("TeacherWithoutCoursesAssigned");
            
            return new ResponseDTO<List<TeacherCoursesDTO>>
            {
                Message = "¡Cursos del docente cargados con éxito!",
                Object = courses,
                StatusCode = HttpStatusCode.OK
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al intentar obtener los cursos del docente con id ({teacherId}): " + e.Message);

            if (e.Message.Contains("TeacherWithoutCoursesAssigned"))
                return new ResponseDTO<List<TeacherCoursesDTO>>()
                {
                    Message = "¡Docente sin cursos asignados!",
                    Object = null,
                    StatusCode = HttpStatusCode.NotFound
                };

            return new ResponseDTO<List<TeacherCoursesDTO>>()
            {
                Message = "¡Error al intentar obtener los cursos del docente!",
                Object = null,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    public async Task<ResponseDTO<List<ScheduleDTO>>> GetScheduleByDivisionId(long id)
    {
        try
        {
            return new ResponseDTO<List<ScheduleDTO>>
            {
                Message = "¡Horarios de la divisón obtenidos con éxito!",
                Object = await bbdd.Schedules.Where(s => s.Division!.DivisionTemplateId == id).Select(s => new ScheduleDTO
                {
                    Day = s.Day,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                }).ToListAsync(),
                StatusCode = HttpStatusCode.OK
            };
        }
        catch(Exception e)
        {
            Console.WriteLine($"Error al intentar obtener el horario de la división: " + e.Message);
            
            return new ResponseDTO<List<ScheduleDTO>>()
            {
                Message = "¡Error al intentar obtener el horario de la división!",
                Object = null,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }
}