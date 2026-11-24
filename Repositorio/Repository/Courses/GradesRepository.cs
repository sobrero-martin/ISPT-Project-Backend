using BD;
using BD.Entidades;
using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Courses;
using System.Net;

namespace Repositorio.Repository.Courses;

public class GradesRepository : IGradesRepository
{
    private readonly AppDbContext db;
    public GradesRepository(AppDbContext db) => this.db = db;

    public async Task<ResponseDTO<List<CourseGradeDTO>>> GetGrades(long evaluationId)
    {
        var evaluation = await db.DivisionExams.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == evaluationId && e.state);
        if (evaluation == null)
            return Error<List<CourseGradeDTO>>(HttpStatusCode.NotFound, "No se encontró la instancia evaluativa.");

        var rows = await db.FileDivisions.AsNoTracking()
            .Where(fd => fd.DivisionId == evaluation.DivisionId && fd.state && fd.FileDivisionAdmitedStatus == DTO.ENUM.EnumFileDivisionAdmitedStatus.Aceptado)
            .OrderBy(fd => fd.FileCurriculum.File.Code)
            .Select(fd => new CourseGradeDTO
            {
                FileDivisionId = fd.Id,
                FileCode = fd.FileCurriculum.File.Code,
                StudentName = fd.FileCurriculum.File.Student.Lastname + ", " + fd.FileCurriculum.File.Student.Firstname,
                Grade = db.Grades.Where(g => g.DivisionExamId == evaluationId && g.FileDivisionId == fd.Id && g.state)
                    .Select(g => (short?)g.GradeNumber).FirstOrDefault()
            }).ToListAsync();

        return Ok(rows, "Alumnos y notas cargados.");
    }

    public async Task<ResponseDTO<string>> SaveGrades(long evaluationId, List<CourseGradeUpdateDTO> grades)
    {
        var evaluation = await db.DivisionExams.FirstOrDefaultAsync(e => e.Id == evaluationId && e.state);
        if (evaluation == null)
            return Error<string>(HttpStatusCode.NotFound, "No se encontró la instancia evaluativa.");

        var enrollmentIds = await db.FileDivisions
            .Where(fd => fd.DivisionId == evaluation.DivisionId && fd.state && fd.FileDivisionAdmitedStatus == DTO.ENUM.EnumFileDivisionAdmitedStatus.Aceptado)
            .Select(fd => fd.Id).ToListAsync();
        var existingGrades = await db.Grades.Where(g => g.DivisionExamId == evaluationId && g.state).ToListAsync();

        foreach (var update in grades)
        {
            if (!enrollmentIds.Contains(update.FileDivisionId) || (update.Grade.HasValue && (update.Grade < 1 || update.Grade > 10)))
                return Error<string>(HttpStatusCode.BadRequest, "Una de las notas o inscripciones enviadas no es válida. Las notas deben estar entre 1 y 10.");
        }

        foreach (var update in grades)
        {
            var existing = existingGrades.FirstOrDefault(g => g.FileDivisionId == update.FileDivisionId);
            if (update.Grade is null)
            {
                continue;
            }

            if (existing != null)
                existing.GradeNumber = update.Grade.Value;
            else
                db.Grades.Add(new Grade { FileDivisionId = update.FileDivisionId, DivisionExamId = evaluationId, GradeNumber = update.Grade.Value, state = true });
        }

        await db.SaveChangesAsync();
        return Ok("Notas guardadas.", "Notas guardadas correctamente.");
    }

    private static ResponseDTO<T> Ok<T>(T value, string message) => new() { Object = value, Message = message, StatusCode = HttpStatusCode.OK };
    private static ResponseDTO<T> Error<T>(HttpStatusCode status, string message) => new() { Object = default!, Message = message, StatusCode = status };
}
