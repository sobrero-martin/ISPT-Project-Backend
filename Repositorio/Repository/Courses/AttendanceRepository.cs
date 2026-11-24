using BD;
using BD.Entidades;
using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Assistance;
using System.Net;

namespace Repositorio.Repository.Assitance;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext db;
    public AttendanceRepository(AppDbContext db) => this.db = db;

    public async Task<ResponseDTO<List<AttendanceDayDTO>>> GetDays(long divisionId)
    {
        var days = await db.AttendanceDays.Where(d => d.DivisionId == divisionId)
            .OrderByDescending(d => d.Date)
            .Select(d => new AttendanceDayDTO { Id = d.Id, Date = d.Date,
                Total = d.Attendances.Count(a => a.state),
                Present = d.Attendances.Count(a => a.state && a.Status),
                State = d.Attendances.Any(a => a.state) && d.Attendances.Where(a => a.state).All(a => a.Recorded) ? "Completa" : "Pendiente" })
            .ToListAsync();
        return Ok(days, "Días de asistencia cargados.");
    }

    public async Task<ResponseDTO<AttendanceDayDTO>> CreateToday(long divisionId, DateTime date)
    {
        var divisionExists = await db.Divisions.AnyAsync(d => d.Id == divisionId);
        if (!divisionExists) return Error<AttendanceDayDTO>(HttpStatusCode.NotFound, "No se encontró el curso.");
        var today = date.Date;
        var existing = await db.AttendanceDays.FirstOrDefaultAsync(d => d.DivisionId == divisionId && d.Date.Date == today);
        if (existing != null) return Ok(new AttendanceDayDTO { Id = existing.Id, Date = existing.Date }, "El día de hoy ya estaba cargado.");

        var day = new AttendanceDay { DivisionId = divisionId, Date = today, Status = false, Attendances = new List<Attendance>() };
        db.AttendanceDays.Add(day);
        var enrollments = db.FileDivisions.Where(fd => fd.DivisionId == divisionId && fd.FileDivisionAdmitedStatus == DTO.ENUM.EnumFileDivisionAdmitedStatus.Aceptado);
        await foreach (var enrollment in enrollments.AsAsyncEnumerable())
            day.Attendances.Add(new Attendance { FileDivisionId = enrollment.Id, Status = false, Justified = false, Recorded = false });
        await db.SaveChangesAsync();
        return Ok(new AttendanceDayDTO { Id = day.Id, Date = day.Date, Total = day.Attendances.Count, State = "Pendiente" }, "Día de asistencia creado.");
    }

    public async Task<ResponseDTO<List<DailyAttendanceDTO>>> GetDaily(long attendanceDayId)
    {
        var day = await db.AttendanceDays.Where(d => d.Id == attendanceDayId).Select(d => d.Id).FirstOrDefaultAsync();
        if (day == 0) return Error<List<DailyAttendanceDTO>>(HttpStatusCode.NotFound, "No se encontró el día de asistencia.");
        var rows = await db.Attendances.Where(a => a.AttendanceDayId == attendanceDayId && a.state)
            .OrderBy(a => a.FileDivision.FileCurriculum.File.Code)
            .Select(a => new DailyAttendanceDTO {
                FileDivisionId = a.FileDivisionId,
                FileCode = a.FileDivision.FileCurriculum.File.Code,
                StudentName = a.FileDivision.FileCurriculum.File.Student.Lastname + ", " + a.FileDivision.FileCurriculum.File.Student.Firstname,
                Status = !a.Recorded ? "Pendiente" : a.Justified ? "Justificado" : a.Status ? "Presente" : "Ausente"
            }).ToListAsync();
        return Ok(rows, "Asistencia diaria cargada.");
    }

    public async Task<ResponseDTO<string>> UpdateDaily(long attendanceDayId, List<AttendanceUpdateDTO> attendances)
    {
        var rows = await db.Attendances.Where(a => a.AttendanceDayId == attendanceDayId && a.state).ToListAsync();
        if (rows.Count == 0 && !await db.AttendanceDays.AnyAsync(d => d.Id == attendanceDayId))
            return Error<string>(HttpStatusCode.NotFound, "No se encontró el día de asistencia.");
        foreach (var update in attendances)
        {
            var row = rows.FirstOrDefault(a => a.FileDivisionId == update.FileDivisionId);
            if (row == null || !new[] { "Presente", "Ausente", "Justificado" }.Contains(update.Status))
                return Error<string>(HttpStatusCode.BadRequest, "La asistencia enviada no es válida.");
            row.Status = update.Status == "Presente";
            row.Justified = update.Status == "Justificado";
            row.Recorded = true;
        }
        await db.SaveChangesAsync();
        return Ok("Guardado", "Asistencia actualizada.");
    }

    private static ResponseDTO<T> Ok<T>(T value, string message) => new() { Object = value, Message = message, StatusCode = HttpStatusCode.OK };
    private static ResponseDTO<T> Error<T>(HttpStatusCode status, string message) => new() { Object = default!, Message = message, StatusCode = status };
}
