using BD;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.CoursesDTO;
using DTO.DTOs.CoursesDTO.InscriptionForm;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using DTO.ENUM;
using DTO.DTOs.StudentsDTO;
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

    public async Task<ResponseDTO<List<CareerInscriptionDTO>>> GetAllAvailableCareerInscription(long studentId)
    {
        try
        {
            var activeCurriculumIds = await bbdd.FileCurriculum
                .AsNoTracking()
                .Where(fc => fc.state && fc.File.state && fc.File.StudentId == studentId && fc.File.Status == EnumStudentStatus.Activo)
                .Select(fc => fc.CurriculumId)
                .Distinct()
                .ToListAsync();

            if (activeCurriculumIds.Count == 0)
                return new ResponseDTO<List<CareerInscriptionDTO>>
                {
                    Message = "No tenés un legajo activo habilitado para inscribirte.",
                    Object = new List<CareerInscriptionDTO>(),
                    StatusCode = HttpStatusCode.OK
                };

            var careers = await bbdd.SchoolYears
                .Where(sy => activeCurriculumIds.Contains(sy.CurriculumId) && sy.StartDate.Date <= DateTime.Today && DateTime.Today <= sy.EndDate.Date)
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
            var activeFileCurriculum = await bbdd.FileCurriculum
                .AsNoTracking()
                .FirstOrDefaultAsync(fc => fc.state && fc.File.state && fc.File.StudentId == studentId &&
                    fc.CurriculumId == curriculumId && fc.File.Status == EnumStudentStatus.Activo);

            if (activeFileCurriculum == null)
                return new ResponseDTO<InscriptionFormDTO>
                {
                    Message = "No tenés un legajo activo habilitado para este plan de estudios.",
                    Object = null,
                    StatusCode = HttpStatusCode.Forbidden
                };

            var cicloActuales = await bbdd.SchoolYears
                .Where(sy => sy.CurriculumId == curriculumId && sy.SchoolYearNumber == schoolYear).ToListAsync();
            var divisionesActuales = await bbdd.Divisions.Where(d => cicloActuales.Select(c => c.Id).Contains(d.SchoolYearId))
                .Include(d => d.DivisionTemplate).ToListAsync();

            var subjectRequests = await bbdd.FileDivisions
                .AsNoTracking()
                .Where(fd => fd.FileCurriculumId == activeFileCurriculum.Id && fd.state)
                .Select(fd => new
                {
                    SubjectId = fd.Division!.DivisionTemplate!.SubjectId,
                    fd.FileDivisionAdmitedStatus
                })
                .ToListAsync();
            var requestStatusBySubject = subjectRequests
                .GroupBy(request => request.SubjectId)
                .ToDictionary(group => group.Key, group =>
                    group.Any(request => request.FileDivisionAdmitedStatus == EnumFileDivisionAdmitedStatus.Pendiente)
                        ? EnumInscriptionSubjectStudentStatus.Requested
                        : group.Any(request => request.FileDivisionAdmitedStatus == EnumFileDivisionAdmitedStatus.Aceptado)
                            ? EnumInscriptionSubjectStudentStatus.Enrolled
                            : EnumInscriptionSubjectStudentStatus.RejectedRequest);

            var espaciosCurriculares = await bbdd.Subjects.Where(s => s.CurriculumId == curriculumId && 
            cicloActuales.Select(c => c.YearNumber).Contains(s.Year)).ToListAsync();

            var subjectIds = espaciosCurriculares.Select(s => s.Id).ToList();
            var correlatives = await bbdd.Correlatives
                .AsNoTracking()
                .Where(c => c.state && subjectIds.Contains(c.SubjectId))
                .Select(c => new
                {
                    c.SubjectId,
                    c.SubjectCorrelativeId,
                    c.IsMandatory,
                    CorrelativeName = c.SubjectCorrelative!.Name
                })
                .ToListAsync();

            var fileDivisions = await bbdd.FileDivisions
                .AsNoTracking()
                .Where(fd => fd.FileCurriculumId == activeFileCurriculum.Id && fd.state && fd.FileDivisionAdmitedStatus == EnumFileDivisionAdmitedStatus.Aceptado)
                .Select(fd => new
                {
                    fd.Id,
                    SubjectId = fd.Division!.DivisionTemplate!.SubjectId,
                    fd.StudentFileSubjectStatus,
                    fd.StudentSubjectDateExpiration
                })
                .ToListAsync();
            var fileDivisionIds = fileDivisions.Select(fd => fd.Id).ToList();

            var gradeSummaries = await bbdd.Grades
                .AsNoTracking()
                .Where(g => g.state && fileDivisionIds.Contains(g.FileDivisionId))
                .GroupBy(g => g.FileDivisionId)
                .Select(g => new { FileDivisionId = g.Key, Count = g.Count(), LowestGrade = g.Min(x => x.GradeNumber) })
                .ToDictionaryAsync(g => g.FileDivisionId);

            var attendanceSummaries = await bbdd.Attendances
                .AsNoTracking()
                .Where(a => a.state && fileDivisionIds.Contains(a.FileDivisionId))
                .Where(a => a.Status != EnumAttendanceStatus.Pendiente)
                .GroupBy(a => a.FileDivisionId)
                .Select(g => new { FileDivisionId = g.Key, Count = g.Count(), Present = g.Count(a => a.Status == EnumAttendanceStatus.Presente) })
                .ToDictionaryAsync(a => a.FileDivisionId);

            var regularSubjectIds = fileDivisions
                .Where(fd => fd.StudentFileSubjectStatus != EnumStudentFileSubjectStatus.Libre
                    && (!fd.StudentSubjectDateExpiration.HasValue || fd.StudentSubjectDateExpiration.Value.Date >= DateTime.Today)
                    && gradeSummaries.TryGetValue(fd.Id, out var grades) && grades.Count > 0 && grades.LowestGrade >= 4
                    && attendanceSummaries.TryGetValue(fd.Id, out var attendance) && attendance.Count > 0
                    && attendance.Present * 100d / attendance.Count >= 75)
                .Select(fd => fd.SubjectId)
                .ToHashSet();

            var approvedSubjectIds = fileDivisions
                .Where(fd => fd.StudentFileSubjectStatus == EnumStudentFileSubjectStatus.Aprobado)
                .Select(fd => fd.SubjectId)
                .ToHashSet();

            var finalExamApprovedSubjects = await bbdd.FinalExamGrades
                .AsNoTracking()
                .Where(grade => grade.state && grade.GradeState == EnumFinalGradeForIEFIandFExam.Aprobado
                    && grade.FileDivision!.state && grade.FileDivision.FileCurriculumId == activeFileCurriculum.Id
                    && grade.FinalExam!.state)
                .Select(grade => grade.FinalExam!.SubjectId)
                .Distinct()
                .ToListAsync();
            approvedSubjectIds.UnionWith(finalExamApprovedSubjects);

            var iefiApprovedSubjects = await bbdd.IEFIGrades
                .AsNoTracking()
                .Where(grade => grade.GradeState == EnumFinalGradeForIEFIandFExam.Aprobado
                    && grade.FileDivision!.state && grade.FileDivision.FileCurriculumId == activeFileCurriculum.Id
                    && grade.IEFI!.state && grade.IEFI.Division!.state)
                .Select(grade => grade.IEFI!.Division!.DivisionTemplate!.SubjectId)
                .Distinct()
                .ToListAsync();
            approvedSubjectIds.UnionWith(iefiApprovedSubjects);

            var correlativesBySubject = correlatives.GroupBy(c => c.SubjectId)
                .ToDictionary(group => group.Key, group => group
                    .Select(c => (SubjectCorrelativeId: c.SubjectCorrelativeId, IsMandatory: c.IsMandatory, CorrelativeName: c.CorrelativeName))
                    .ToList());

            return new ResponseDTO<InscriptionFormDTO>
            {
                Message = "Espacios curriculares habilitados según las correlatividades del legajo.",
                Object = new InscriptionFormDTO
                {
                    YearsAvailable = cicloActuales.Count,
                    Subjects = espaciosCurriculares.Select(subject =>
                    {
                        var subjectCorrelatives = correlativesBySubject.TryGetValue(subject.Id, out var foundCorrelatives)
                            ? foundCorrelatives
                            : new List<(long SubjectCorrelativeId, bool IsMandatory, string CorrelativeName)>();
                        var missingCorrelatives = subjectCorrelatives
                            .Where(c =>
                            {
                                var isApproved = approvedSubjectIds.Contains(c.SubjectCorrelativeId);
                                var isRegular = regularSubjectIds.Contains(c.SubjectCorrelativeId);

                                // Una correlativa obligatoria exige aprobación; la regularidad solo alcanza para una no obligatoria.
                                return c.IsMandatory ? !isApproved : !isApproved && !isRegular;
                            })
                            .Select(c => c.CorrelativeName)
                            .ToList();

                        var isApproved = approvedSubjectIds.Contains(subject.Id);
                        var hasPreviousRequest = requestStatusBySubject.TryGetValue(subject.Id, out var requestStatus);
                        var isAvailable = !isApproved && missingCorrelatives.Count == 0;

                        return new SubjectsInscriptionDTO
                        {
                            SubjectId = subject.Id,
                            IsAvailable = isApproved
                                ? EnumInscriptionSubjectStudentStatus.Approved
                                : hasPreviousRequest
                                    ? requestStatus
                                    : isAvailable
                                        ? EnumInscriptionSubjectStudentStatus.Available
                                        : EnumInscriptionSubjectStudentStatus.Unavailable,
                            MissingCorrelatives = missingCorrelatives,
                            SubjectName = subject.Name,
                            YearAvailable = subject.Year,
                            Divisions = divisionesActuales.Where(d => d.DivisionTemplate!.SubjectId == subject.Id)
                                .Select(d => new DivisionInscriptionDTO
                                {
                                    DivisionId = d.Id,
                                    DivisionName = d.DivisionTemplate!.Name
                                }).ToList()
                        };
                    }).ToList()
                },
                StatusCode = HttpStatusCode.OK
            };
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

    public async Task<ResponseDTO<string>> Inscription(StudentInscriptionDTO inscription, long studentId)
    {
        try
        {
            await using var transaction = await bbdd.Database.BeginTransactionAsync();
            if (inscription?.DivisionIds == null)
                return new ResponseDTO<string> { StatusCode = HttpStatusCode.BadRequest, Message = "La solicitud no contiene comisiones para inscribirse.", Object = null };

            var divisionIds = inscription.DivisionIds.Distinct().ToList();
            if (divisionIds.Count == 0)
                return new ResponseDTO<string> { StatusCode = HttpStatusCode.BadRequest, Message = "Seleccioná al menos una comisión para enviar la solicitud.", Object = null };

            if (divisionIds.Count != inscription.DivisionIds.Count)
                return new ResponseDTO<string> { StatusCode = HttpStatusCode.BadRequest, Message = "La solicitud contiene comisiones repetidas.", Object = null };

            var divisions = await bbdd.Divisions
                .AsNoTracking()
                .Where(d => divisionIds.Contains(d.Id) && d.state)
                .Select(d => new
                {
                    d.Id,
                    d.SchoolYearId,
                    CurriculumId = d.SchoolYear!.CurriculumId,
                    SchoolYearNumber = d.SchoolYear.SchoolYearNumber,
                    d.SchoolYear.StartDate,
                    d.SchoolYear.EndDate,
                    SubjectId = d.DivisionTemplate!.SubjectId
                })
                .ToListAsync();

            if (divisions.Count != divisionIds.Count)
                return new ResponseDTO<string> { StatusCode = HttpStatusCode.NotFound, Message = "Una o más comisiones seleccionadas ya no existen.", Object = null };

            if (divisions.GroupBy(d => d.SubjectId).Any(group => group.Count() > 1))
                return new ResponseDTO<string> { StatusCode = HttpStatusCode.BadRequest, Message = "Solo podés seleccionar una comisión por espacio curricular.", Object = null };

            var curriculumIds = divisions.Select(d => d.CurriculumId).Distinct().ToList();
            var fileCurricula = await bbdd.FileCurriculum
                .Where(fc => fc.state && fc.File.state && fc.File.StudentId == studentId && fc.File.Status == EnumStudentStatus.Activo
                    && curriculumIds.Contains(fc.CurriculumId))
                .ToDictionaryAsync(fc => fc.CurriculumId);

            var fileCurriculumIds = fileCurricula.Values.Select(fc => fc.Id).ToList();
            var requestedSubjectIds = divisions.Select(division => division.SubjectId).Distinct().ToList();
            var previousSubjectRequests = await bbdd.FileDivisions
                .AsNoTracking()
                .Where(fd => fd.state && fileCurriculumIds.Contains(fd.FileCurriculumId)
                    && requestedSubjectIds.Contains(fd.Division!.DivisionTemplate!.SubjectId))
                .Select(fd => fd.Division!.DivisionTemplate!.SubjectId)
                .Distinct()
                .ToListAsync();
            if (previousSubjectRequests.Count > 0)
                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "Ya existe una solicitud activa o una decisión de preceptoría para uno de los espacios curriculares seleccionados.",
                    Object = null
                };

            var availableSubjectsBySchoolYear = new Dictionary<(long CurriculumId, int SchoolYearNumber), InscriptionFormDTO>();
            foreach (var division in divisions)
            {
                if (division.StartDate.Date > DateTime.Today || DateTime.Today > division.EndDate.Date)
                    return new ResponseDTO<string> { StatusCode = HttpStatusCode.Locked, Message = "El período de inscripción para una de las comisiones seleccionadas está cerrado.", Object = null };

                if (!fileCurricula.ContainsKey(division.CurriculumId))
                    return new ResponseDTO<string> { StatusCode = HttpStatusCode.Forbidden, Message = "No tenés un legajo activo para una de las carreras seleccionadas.", Object = null };

                var key = (division.CurriculumId, division.SchoolYearNumber);
                if (!availableSubjectsBySchoolYear.TryGetValue(key, out var form))
                {
                    var formResponse = await GetAllSubjectsFromCareer(division.CurriculumId, division.SchoolYearNumber, studentId);
                    if (formResponse.StatusCode != HttpStatusCode.OK || formResponse.Object == null)
                        return new ResponseDTO<string> { StatusCode = formResponse.StatusCode, Message = formResponse.Message, Object = null };
                    form = formResponse.Object;
                    availableSubjectsBySchoolYear[key] = form;
                }

                var requestedSubject = form.Subjects.FirstOrDefault(subject => subject.Divisions.Any(option => option.DivisionId == division.Id));
                if (requestedSubject == null || requestedSubject.IsAvailable != EnumInscriptionSubjectStudentStatus.Available)
                    return new ResponseDTO<string> { StatusCode = HttpStatusCode.BadRequest, Message = "Un espacio curricular seleccionado ya no cumple los requisitos de inscripción.", Object = null };
            }

            foreach (var division in divisions)
            {
                var fileCurriculum = fileCurricula[division.CurriculumId];
                bbdd.FileDivisions.Add(new BD.Entidades.FileDivision
                {
                    FileCurriculumId = fileCurriculum.Id,
                    DivisionId = division.Id,
                    FileDivisionAdmitedStatus = EnumFileDivisionAdmitedStatus.Pendiente,
                    StudentFileSubjectStatus = EnumStudentFileSubjectStatus.SinCursar
                });
            }

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ResponseDTO<string> { StatusCode = HttpStatusCode.Created, Message = "Solicitud de inscripción enviada a preceptoría.", Object = "Solicitud enviada." };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al enviar la solicitud de inscripción: {e}");

            return new ResponseDTO<string>()
            {
                Message = "¡Error al enviar la solicitud de inscripción! Si el error persiste, contactá con preceptoría.",
                Object = null,
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }





    public async Task<ResponseDTO<List<StudentSubjectDTO>>> GetCoursesByStudentId(long fileId)
    {
        try
        {
            var fileExists = await bbdd.Files.AsNoTracking().AnyAsync(file => file.Id == fileId && file.state);
            if (!fileExists)
                return new ResponseDTO<List<StudentSubjectDTO>>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "No se encontró el legajo seleccionado.",
                    Object = null
                };

            var enrollments = await bbdd.FileDivisions
                .AsNoTracking()
                .Where(fd => fd.state
                    && fd.FileDivisionAdmitedStatus == EnumFileDivisionAdmitedStatus.Aceptado
                    && fd.FileCurriculum.state
                    && fd.FileCurriculum.FileId == fileId
                    && fd.Division != null && fd.Division.state
                    && fd.Division.DivisionTemplate != null && fd.Division.DivisionTemplate.TemplateState
                    && fd.Division.DivisionTemplate.Subject != null && fd.Division.DivisionTemplate.Subject.state
                    && fd.Division.SchoolYear != null && fd.Division.SchoolYear.state)
                .Select(fd => new
                {
                    fd.Id,
                    DivisionId = fd.DivisionId,
                    fd.StudentFileSubjectStatus,
                    Subject = fd.Division!.DivisionTemplate!.Subject!.Name,
                    SchoolYear = fd.Division.SchoolYear!.SchoolYearNumber,
                    Division = fd.Division.DivisionTemplate.Name
                })
                .ToListAsync();

            var results = new List<StudentSubjectDTO>();
            foreach (var enrollment in enrollments)
            {
                var grades = await bbdd.Grades.AsNoTracking()
                    .Where(grade => grade.state && grade.FileDivisionId == enrollment.Id)
                    .Select(grade => (double)grade.GradeNumber)
                    .ToListAsync();
                var attendance = await bbdd.Attendances.AsNoTracking()
                    .Where(item => item.state && item.AttendanceDay.state && item.FileDivisionId == enrollment.Id
                        && item.Status != EnumAttendanceStatus.Pendiente)
                    .Select(item => item.Status)
                    .ToListAsync();

                results.Add(new StudentSubjectDTO
                {
                    FileDivisionId = enrollment.Id,
                    DivisionId = enrollment.DivisionId,
                    Subject = enrollment.Subject,
                    SchoolYear = enrollment.SchoolYear,
                    Division = enrollment.Division,
                    Average = grades.Count == 0 ? null : Math.Round(grades.Average(), 2),
                    AttendancePercentage = attendance.Count == 0 ? null : Math.Round(attendance.Count(status => status == EnumAttendanceStatus.Presente) * 100d / attendance.Count, 2),
                    Status = enrollment.StudentFileSubjectStatus.ToString()
                });
            }

            return new ResponseDTO<List<StudentSubjectDTO>>
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Espacios curriculares del legajo obtenidos con éxito.",
                Object = results
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al obtener los espacios curriculares del legajo: {e}");
            return new ResponseDTO<List<StudentSubjectDTO>>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Hubo un error al obtener los espacios curriculares del legajo.",
                Object = null
            };
        }
    }
}
