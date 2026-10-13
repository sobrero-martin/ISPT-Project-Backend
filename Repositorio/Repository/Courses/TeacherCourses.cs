using System.Net;
using BD;
using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.EntityFrameworkCore;

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
                .Join(
                    bbdd.Divisions.AsNoTracking(),
                    td => td.DivisionId,              
                    d => d.Id,                      
                    (td, d) => new TeacherCoursesDTO     
                    {
                        DivisionId = td.DivisionId,
                        Division = td.Division.Name,   
                        Subject = td.Division.Subject.Name,
                        SchoolYear = d.SchoolYear.SchoolYearNumber,      
                        Career = td.Division.Subject.Curriculum.Career.Name
                    }
                )
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
}