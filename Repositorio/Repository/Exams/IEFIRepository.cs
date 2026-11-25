using BD;
using BD.Entities;
using DTO.DTOs.DTO_Response;
using DTO.ENUM;
using DTO.DTOs.ExamDTO;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Exams;
using System.Net;

namespace Repositorio.Repository.Exams
{
    public class IEFIRepository : IIEFIRepository
    {
        private readonly AppDbContext context;

        public IEFIRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<List<IEFIDivisionStudentDTO>>> GetStudents(long divisionId, long? studentPersonId = null, bool registerEligible = false)
        {
            try
            {
                var iefi = await context.IEFIs.FirstOrDefaultAsync(x => x.DivisionId == divisionId && x.state);
                var enrollments = await context.FileDivisions.AsNoTracking()
                    .Where(fd => fd.DivisionId == divisionId && fd.state && fd.FileDivisionAdmitedStatus == EnumFileDivisionAdmitedStatus.Aceptado && (!studentPersonId.HasValue || fd.FileCurriculum.File.StudentId == studentPersonId.Value))
                    .Select(fd => new { fd.Id, fd.FileCurriculum.File.Code, fd.FileCurriculum.File.Student.Firstname, fd.FileCurriculum.File.Student.Lastname })
                    .ToListAsync();

                var result = new List<IEFIDivisionStudentDTO>();
                var changesMade = false;

                foreach (var student in enrollments)
                {
                    var grades = await context.Grades.AsNoTracking().Where(g => g.FileDivisionId == student.Id && g.state).Select(g => (double)g.GradeNumber).ToListAsync();
                    var attendance = await context.Attendances.AsNoTracking()
                        .Where(a => a.FileDivisionId == student.Id && a.state && a.AttendanceDay.Status && a.Status != EnumAttendanceStatus.Pendiente)
                        .Select(a => a.Status).ToListAsync();
                    var ieGrade = iefi == null ? null : await context.IEFIGrades.AsNoTracking().FirstOrDefaultAsync(g => g.IEFIId == iefi.Id && g.FileDivisionId == student.Id);
                    var average = grades.Count == 0 ? (double?)null : Math.Round(grades.Average(), 2);
                    var percentage = attendance.Count == 0 ? (double?)null : Math.Round(attendance.Count(a => a == EnumAttendanceStatus.Presente) * 100d / attendance.Count, 2);
                    if (registerEligible && average >= 7 && percentage >= 75 && ieGrade == null)
                    {
                        if (iefi == null)
                        {
                            iefi = new IEFI { DivisionId = divisionId, Date = DateTime.Today, Time = TimeOnly.MinValue };
                            context.IEFIs.Add(iefi);
                            await context.SaveChangesAsync();
                        }
                        context.IEFIGrades.Add(new IEFIGrade { IEFIId = iefi.Id, FileDivisionId = student.Id });
                        ieGrade = new IEFIGrade { IEFIId = iefi.Id, FileDivisionId = student.Id };
                        changesMade = true;
                    }
                    result.Add(new IEFIDivisionStudentDTO
                    {
                        FileDivisionId = student.Id,
                        FileCode = student.Code,
                        FirstName = student.Firstname,
                        LastName = student.Lastname,
                        Average = average,
                        AttendancePercentage = percentage,
                        Eligible = average >= 7 && percentage >= 75,
                        Registered = ieGrade != null,
                        Grade = ieGrade?.Grade,
                        GradeState = ieGrade?.GradeState.ToString()
                    });
                }

                if (changesMade) await context.SaveChangesAsync();

                return new ResponseDTO<List<IEFIDivisionStudentDTO>> { StatusCode = HttpStatusCode.OK, Message = "Estudiantes de IEFI obtenidos con éxito.", Object = result };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener estudiantes de IEFI: {ex.Message}");
                return new ResponseDTO<List<IEFIDivisionStudentDTO>> { StatusCode = HttpStatusCode.InternalServerError, Message = "Error al obtener estudiantes de IEFI.", Object = null };
            }
        }

        public async Task<ResponseDTO<string>> RegisterStudent(long divisionId, long fileDivisionId)
        {
            try
            {
                var enrollment = await context.FileDivisions.Include(fd => fd.Grades).Include(fd => fd.Attendances).ThenInclude(a => a.AttendanceDay)
                    .FirstOrDefaultAsync(fd => fd.Id == fileDivisionId && fd.DivisionId == divisionId && fd.state && fd.FileDivisionAdmitedStatus == EnumFileDivisionAdmitedStatus.Aceptado);

                if (enrollment == null)
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "No se encontró la inscripción del estudiante.",
                        Object = null
                    };

                var iefi = await context.IEFIs.FirstOrDefaultAsync(x => x.DivisionId == divisionId && x.state);
                if (iefi == null)
                {
                    iefi = new IEFI
                    {
                        DivisionId = divisionId,
                        Date = DateTime.Today,
                        Time = TimeOnly.MinValue
                    };
                    context.IEFIs.Add(iefi);
                    await context.SaveChangesAsync();
                }

                if (await context.IEFIGrades.AnyAsync(g => g.IEFIId == iefi.Id && g.FileDivisionId == fileDivisionId))
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        Message = "El estudiante ya está inscripto al IEFI.",
                        Object = null
                    };

                context.IEFIGrades.Add(new IEFIGrade
                {
                    IEFIId = iefi.Id,
                    FileDivisionId = fileDivisionId,
                    Grade = null,
                    GradeState = null
                });
                await context.SaveChangesAsync();
                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "Estudiante inscripto al IEFI con éxito.",
                    Object = "Inscripción registrada."
                };
            }
            catch (Exception ex) { Console.WriteLine($"Error al inscribir estudiante al IEFI: {ex.Message}"); return new ResponseDTO<string> { StatusCode = HttpStatusCode.InternalServerError, Message = "Error al inscribir al estudiante.", Object = null }; }
        }

        public async Task<ResponseDTO<string>> SaveGrade(long divisionId, long fileDivisionId, int? grade)
        {
            try
            {
                if (grade.HasValue && (grade < 1 || grade > 10)) return new ResponseDTO<string> { StatusCode = HttpStatusCode.BadRequest, Message = "La nota debe estar entre 1 y 10, o vacía si no asistió.", Object = null };
                var iefi = await context.IEFIs.FirstOrDefaultAsync(x => x.DivisionId == divisionId && x.state);
                var result = iefi == null ? null : await context.IEFIGrades.FirstOrDefaultAsync(g => g.IEFIId == iefi.Id && g.FileDivisionId == fileDivisionId);
                if (result == null) return new ResponseDTO<string> { StatusCode = HttpStatusCode.NotFound, Message = "El estudiante no está inscripto al IEFI.", Object = null };
                result.Grade = grade;
                result.GradeState = grade == null ? DTO.ENUM.EnumFinalGradeForIEFIandFExam.NoAsistio : grade >= 4 ? DTO.ENUM.EnumFinalGradeForIEFIandFExam.Aprobado : DTO.ENUM.EnumFinalGradeForIEFIandFExam.Desaprobado;
                await context.SaveChangesAsync();
                return new ResponseDTO<string> { StatusCode = HttpStatusCode.OK, Message = "Nota de IEFI guardada con éxito.", Object = "Nota actualizada." };
            }
            catch (Exception ex) { Console.WriteLine($"Error al guardar nota IEFI: {ex.Message}"); return new ResponseDTO<string> { StatusCode = HttpStatusCode.InternalServerError, Message = "Error al guardar la nota de IEFI.", Object = null }; }
        }

        public async Task<ResponseDTO<List<IEFIDTO>>> GetFull()
        {
            try
            {
                var iefis = await context.Set<IEFI>()
                    .AsNoTracking()
                    .Where(i => i.state)
                    .Select(i => new IEFIDTO
                    {
                        Id = i.Id,
                        DivisionId = i.DivisionId,
                        SubjectName = i.Division!.DivisionTemplate!.Subject!.Name,
                        Date = i.Date,
                        Time = i.Time,
                        RecordBook = i.RecordBook,
                        PageNumber = i.PageNumber
                    })
                    .ToListAsync();

                return new ResponseDTO<List<IEFIDTO>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Object = iefis,
                    Message = "Listado de IEFI obtenido exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el listado de IEFI: {ex.Message}");
                return new ResponseDTO<List<IEFIDTO>>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Error al obtener el listado de IEFI."
                };
            }
        }

        public async Task<ResponseDTO<IEFIPostDTO>> GetIEFIById(long id)
        {
            try
            {
                var iefi = await context.Set<IEFI>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.Id == id && i.state);

                if (iefi == null)
                {
                    return new ResponseDTO<IEFIPostDTO>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "No se encontró el IEFI solicitado.",
                        Object = null
                    };
                }

                return new ResponseDTO<IEFIPostDTO>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Operación exitosa.",
                    Object = new IEFIPostDTO
                    {
                        Id = iefi.Id,
                        CreatedById = iefi.CreatedBy,
                        DivisionId = iefi.DivisionId,
                        Date = iefi.Date,
                        Time = iefi.Time,
                        RecordBook = iefi.RecordBook,
                        PageNumber = iefi.PageNumber
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el IEFI con ID {id}: {ex.Message}");
                return new ResponseDTO<IEFIPostDTO>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al consultar el IEFI.",
                    Object = null
                };
            }
        }

        public async Task<ResponseDTO<string>> Post(IEFIPostDTO iefi)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var iefiEntity = new IEFI
                {
                    Id = iefi.Id,
                    CreatedBy = iefi.CreatedById ?? Guid.Empty,
                    DivisionId = iefi.DivisionId,
                    Date = iefi.Date,
                    Time = iefi.Time,
                    RecordBook = iefi.RecordBook,
                    PageNumber = iefi.PageNumber
                };

                context.Set<IEFI>().Add(iefiEntity);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "Operación exitosa.",
                    Object = $"¡IEFI #{iefiEntity.Id} creado con éxito!"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el IEFI: {ex.Message}");
                await transaction.RollbackAsync();
                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al crear el IEFI.",
                    Object = null
                };
            }
        }

        public async Task<ResponseDTO<string>> Edit(IEFIPostDTO iefi)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingIEFI = await context.Set<IEFI>().FindAsync(iefi.Id);
                if (existingIEFI == null || !existingIEFI.state)
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "No se encontró el IEFI especificado."
                    };
                }

                existingIEFI.DivisionId = iefi.DivisionId;
                existingIEFI.Date = iefi.Date;
                existingIEFI.Time = iefi.Time;
                existingIEFI.RecordBook = iefi.RecordBook;
                existingIEFI.PageNumber = iefi.PageNumber;
                existingIEFI.UpdatedBy = iefi.UpdatedById ?? iefi.CreatedById;

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Operación exitosa.",
                    Object = $"¡IEFI #{iefi.Id} actualizado con éxito!"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el IEFI: {ex.Message}");
                await transaction.RollbackAsync();
                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al actualizar el IEFI.",
                    Object = null
                };
            }
        }

        public async Task<ResponseDTO<string>> Delete(long id)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var iefi = await context.Set<IEFI>().FirstOrDefaultAsync(i => i.Id == id && i.state);
                if (iefi == null)
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "El IEFI que se desea eliminar no existe.",
                        Object = null
                    };
                }

                iefi.state = false;
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "¡Operación exitosa!",
                    Object = "¡IEFI eliminado con éxito!"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el IEFI: {ex.Message}");
                await transaction.RollbackAsync();
                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al eliminar el IEFI.",
                    Object = null
                };
            }
        }
    }
}
