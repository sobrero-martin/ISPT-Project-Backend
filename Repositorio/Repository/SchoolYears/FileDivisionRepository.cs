using BD;
using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using DTO.ENUM;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.SchoolYears;
using System.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Repository.SchoolYears
{
    public class FileDivisionRepository : IFileDivisionRepository
    {
        private readonly AppDbContext context;

        public FileDivisionRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<List<FileDivisionDTO>>> GetByDivisionId(long divisionId)
        {
            try
            {
                var fileDivisions = context.Set<FileDivision>()
                    .Where(fd => fd.DivisionId == divisionId)
                    .Select(fd => new FileDivisionDTO
                    {
                        FileCode = fd.FileCurriculum.File.Code,
                        StudentName = fd.FileCurriculum.File.Student.Lastname + ", " + fd.FileCurriculum.File.Student.Firstname,
                        FileDivisionStatus = fd.FileDivisionAdmitedStatus.ToString(),
                        FileDivisionObservations = fd.FileDivisionObservations
                    })
                    .ToList();


                return new ResponseDTO<List<FileDivisionDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = fileDivisions,
                    Message = "File divisions retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los legajos_divisiones: {ex.Message}");

                return new ResponseDTO<List<FileDivisionDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Error al obtener las inscripciones de estudiantes."
                };
            }
        }

        public async Task<ResponseDTO<string>> Post(StudentFileDivisionPostDTO studentFileDivisionDTO)
        {
            try
            {
                var fileDivisionEntity = new FileDivision
                {
                    FileCurriculumId = studentFileDivisionDTO.FileId,
                    DivisionId = studentFileDivisionDTO.DivisionId,
                    FileDivisionAdmitedStatus = EnumFileDivisionAdmitedStatus.Aceptado,
                    FileDivisionObservations = null,
                    CreatedBy = studentFileDivisionDTO.CreatedBy ?? Guid.Empty,
                };
                context.Set<FileDivision>().Add(fileDivisionEntity);
                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = null,
                    Message = "File division created successfully."    
                };
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error al crear la inscripción de estudiante: {ex.Message}");

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Error al crear la inscripción de estudiante."
                };
            }
        }

        public async Task<ResponseDTO<List<InscriptionRequestDTO>>> GetInscriptionRequests(long schoolYearId)
        {
            try
            {
                var schoolYear = await context.SchoolYears.AsNoTracking()
                    .Where(year => year.Id == schoolYearId && year.state)
                    .Select(year => new { year.CurriculumId, year.SchoolYearNumber })
                    .FirstOrDefaultAsync();
                if (schoolYear == null)
                    return new ResponseDTO<List<InscriptionRequestDTO>> { StatusCode = HttpStatusCode.NotFound, Message = "No se encontró el ciclo lectivo.", Object = null };

                var schoolYearIds = await context.SchoolYears.AsNoTracking()
                    .Where(year => year.CurriculumId == schoolYear.CurriculumId && year.SchoolYearNumber == schoolYear.SchoolYearNumber && year.state)
                    .Select(year => year.Id)
                    .ToListAsync();

                var requests = await context.FileDivisions.AsNoTracking()
                    .Where(request => request.state && schoolYearIds.Contains(request.Division!.SchoolYearId))
                    .Select(request => new
                    {
                        request.FileCurriculumId,
                        FileCode = request.FileCurriculum.File.Code,
                        request.FileCurriculum.File.Student.Firstname,
                        request.FileCurriculum.File.Student.Lastname,
                        request.Id,
                        SubjectName = request.Division!.DivisionTemplate!.Subject!.Name,
                        DivisionName = request.Division.DivisionTemplate.Name,
                        Status = request.FileDivisionAdmitedStatus
                    })
                    .ToListAsync();

                var result = requests.GroupBy(request => new
                    {
                        request.FileCurriculumId,
                        request.FileCode,
                        request.Firstname,
                        request.Lastname
                    })
                    .Select(group => new InscriptionRequestDTO
                    {
                        FileCurriculumId = group.Key.FileCurriculumId,
                        FileCode = group.Key.FileCode,
                        FirstName = group.Key.Firstname,
                        LastName = group.Key.Lastname,
                        PendingDivisionsCount = group.Count(request => request.Status == EnumFileDivisionAdmitedStatus.Pendiente),
                        IsCompleted = group.All(request => request.Status != EnumFileDivisionAdmitedStatus.Pendiente),
                        Divisions = group.Select(request => new InscriptionRequestDivisionDTO
                        {
                            FileDivisionId = request.Id,
                            SubjectName = request.SubjectName,
                            DivisionName = request.DivisionName,
                            Status = request.Status
                        }).ToList()
                    })
                    .ToList();

                return new ResponseDTO<List<InscriptionRequestDTO>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Solicitudes de inscripción obtenidas con éxito.",
                    Object = result
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consultar solicitudes de inscripción: {ex.Message}");
                return new ResponseDTO<List<InscriptionRequestDTO>> { StatusCode = HttpStatusCode.InternalServerError, Message = "Error al obtener las solicitudes.", Object = null };
            }
        }

        public async Task<ResponseDTO<string>> ReviewInscriptionRequests(long schoolYearId, ReviewInscriptionRequestsDTO review)
        {
            try
            {
                await using var transaction = await context.Database.BeginTransactionAsync();
                if (review?.Decisions == null || review.Decisions.Count == 0)
                    return new ResponseDTO<string> { StatusCode = HttpStatusCode.BadRequest, Message = "No hay decisiones para guardar.", Object = null };

                if (review.Decisions.Select(decision => decision.FileDivisionId).Distinct().Count() != review.Decisions.Count
                    || review.Decisions.Any(decision => decision.Status is not (EnumFileDivisionAdmitedStatus.Pendiente or EnumFileDivisionAdmitedStatus.Aceptado or EnumFileDivisionAdmitedStatus.Rechazado)))
                    return new ResponseDTO<string> { StatusCode = HttpStatusCode.BadRequest, Message = "La solicitud contiene decisiones inválidas o repetidas.", Object = null };

                var schoolYear = await context.SchoolYears.AsNoTracking()
                    .Where(year => year.Id == schoolYearId && year.state)
                    .Select(year => new { year.CurriculumId, year.SchoolYearNumber })
                    .FirstOrDefaultAsync();
                if (schoolYear == null)
                    return new ResponseDTO<string> { StatusCode = HttpStatusCode.NotFound, Message = "No se encontró el ciclo lectivo.", Object = null };

                var schoolYearIds = await context.SchoolYears.AsNoTracking()
                    .Where(year => year.CurriculumId == schoolYear.CurriculumId && year.SchoolYearNumber == schoolYear.SchoolYearNumber && year.state)
                    .Select(year => year.Id)
                    .ToListAsync();
                var decisionIds = review.Decisions.Select(decision => decision.FileDivisionId).ToList();
                var requestsToUpdate = await context.FileDivisions
                    .Where(request => request.state && decisionIds.Contains(request.Id)
                        && schoolYearIds.Contains(request.Division!.SchoolYearId))
                    .ToListAsync();

                if (requestsToUpdate.Count != review.Decisions.Count)
                    return new ResponseDTO<string> { StatusCode = HttpStatusCode.Conflict, Message = "Una o más solicitudes ya fueron revisadas o no pertenecen a este ciclo lectivo.", Object = null };

                foreach (var request in requestsToUpdate)
                {
                    request.FileDivisionAdmitedStatus = review.Decisions.First(decision => decision.FileDivisionId == request.Id).Status;
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ResponseDTO<string> { StatusCode = HttpStatusCode.OK, Message = "Cambios de inscripción guardados con éxito.", Object = "Cambios guardados." };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar decisiones de inscripción: {ex.Message}");
                return new ResponseDTO<string> { StatusCode = HttpStatusCode.InternalServerError, Message = "Error al guardar las decisiones de inscripción.", Object = null };
            }
        }
    }
}
