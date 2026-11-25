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
            var courses = await bbdd.Divisions
                .AsNoTracking()
                .Where(d => bbdd.TeacherDivisions.Any(td => td.TeacherId == teacherId && td.DivisionId == d.DivisionTemplateId))
                .Select(d => new TeacherCoursesDTO
                {
                    DivisionId = d.Id,
                    IsClosed = d.DivisionState == "Closed",
                    Division = d.DivisionTemplate!.Name,
                    Subject = d.DivisionTemplate.Subject!.Name,
                    SchoolYear = d.SchoolYear!.SchoolYearNumber,
                    Career = d.DivisionTemplate.Subject.Curriculum!.Career!.Name
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

    public async Task<ResponseDTO<string>> CloseDivision(long divisionId, long teacherId)
    {
        await using var transaction = await bbdd.Database.BeginTransactionAsync();
        var division = await bbdd.Divisions.FirstOrDefaultAsync(d => d.Id == divisionId);
        if (division == null) return new() { StatusCode = HttpStatusCode.NotFound, Message = "No se encontró el curso." };
        if (!await bbdd.TeacherDivisions.AnyAsync(td => td.DivisionId == division.DivisionTemplateId && td.TeacherId == teacherId))
            return new() { StatusCode = HttpStatusCode.Forbidden, Message = "No estás asignado a este curso." };
        if (division.DivisionState == "Closed")
            return new() { StatusCode = HttpStatusCode.Conflict, Message = "El curso ya está cerrado." };

        var enrollments = await bbdd.FileDivisions
            .Where(fd => fd.DivisionId == divisionId && fd.state && fd.FileDivisionAdmitedStatus == DTO.ENUM.EnumFileDivisionAdmitedStatus.Aceptado)
            .ToListAsync();
        var exams = await bbdd.DivisionExams.Where(e => e.DivisionId == divisionId && e.state).Select(e => e.Id).ToListAsync();
        var grades = await bbdd.Grades.Where(g => g.state && exams.Contains(g.DivisionExamId)).ToListAsync();
        var enrollmentIds = enrollments.Select(fd => fd.Id).ToList();
        var attendance = await bbdd.Attendances
            .Where(a => a.state && enrollmentIds.Contains(a.FileDivisionId) && a.Status != DTO.ENUM.EnumAttendanceStatus.Pendiente)
            .ToListAsync();
        var expirationDate = DateTime.Today.AddYears(2);

        foreach (var enrollment in enrollments)
        {
            var studentGrades = grades.Where(g => g.FileDivisionId == enrollment.Id).ToList();
            var studentAttendance = attendance.Where(a => a.FileDivisionId == enrollment.Id).ToList();
            var enoughAttendance = studentAttendance.Count > 0 && studentAttendance.Count(a => a.Status == DTO.ENUM.EnumAttendanceStatus.Presente) * 100d / studentAttendance.Count >= 75;
            var hasAllGrades = exams.Count > 0 && exams.All(examId => studentGrades.Any(g => g.DivisionExamId == examId));
            var gradesForAllExams = exams.Select(examId => studentGrades.FirstOrDefault(g => g.DivisionExamId == examId)?.GradeNumber).ToList();

            enrollment.StudentFileSubjectStatus = enoughAttendance && hasAllGrades && gradesForAllExams.All(g => g >= 7)
                ? DTO.ENUM.EnumStudentFileSubjectStatus.Aprobado
                : enoughAttendance && hasAllGrades && gradesForAllExams.All(g => g >= 4)
                    ? DTO.ENUM.EnumStudentFileSubjectStatus.Regular
                    : DTO.ENUM.EnumStudentFileSubjectStatus.Libre;
            enrollment.StudentSubjectDateExpiration = enrollment.StudentFileSubjectStatus is DTO.ENUM.EnumStudentFileSubjectStatus.Regular
                or DTO.ENUM.EnumStudentFileSubjectStatus.Aprobado
                ? expirationDate
                : null;
        }

        division.DivisionState = "Closed";
        await bbdd.SaveChangesAsync();
        await transaction.CommitAsync();
        return new() { StatusCode = HttpStatusCode.OK, Message = "Curso cerrado y regularidades calculadas.", Object = "Curso cerrado." };
    }

    public async Task<ResponseDTO<List<ScheduleDTO>>> GetScheduleByDivisionId(long id)
    {
        try
        {
            return new ResponseDTO<List<ScheduleDTO>>
            {
                Message = "¡Horarios de la divisón obtenidos con éxito!",
                Object = await bbdd.Schedules.Where(s => s.DivisionId == id).Select(s => new ScheduleDTO
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
