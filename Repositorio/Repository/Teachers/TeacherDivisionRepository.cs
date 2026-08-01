using System.Net;
using BD;
using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.TeachersDTO;
using Microsoft.EntityFrameworkCore;

namespace Repositorio.Repository;

public class TeacherDivisionRepository : ITeacherDivisionRepository
{
    
    private readonly AppDbContext bbdd;

    public TeacherDivisionRepository(AppDbContext bbdd)
    {
        this.bbdd = bbdd;
    }

    public async Task<ResponseDTO<List<TeacherDivisionDTO>>> GetAllTeachersByDivisionId(long divisionId)
    {
        try
        {
            var list = await bbdd.TeacherDivisions.Where(t => t.DivisionId == divisionId)
                .Include(td => td.Teacher)
                .Select(td => new TeacherDivisionDTO()
                {
                    Id = td.Id,
                    DNI = td.Teacher.DocumentNumber,
                    TeacherFullname = $"{td.Teacher.Firstname} {td.Teacher.Lastname}",
                    TeacherStatus = td.TeacherStatus.ToString(),
                    StartDate = td.StartDate,
                    EndDate = td.EndDate,
                }).ToListAsync();

            return new ResponseDTO<List<TeacherDivisionDTO>>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = list
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al intentar cargar los docentes de la división: " + e.Message);
            
            return new ResponseDTO<List<TeacherDivisionDTO>>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar cargar los docentes de la división!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<TeacherDivisionPostDTO>> GetTeacherDivision(long teacherDivisionId)
    {
        try
        {
            var teacherDivision = await bbdd.TeacherDivisions.FirstOrDefaultAsync(t => t.Id == teacherDivisionId);
            if(teacherDivision == null) throw new Exception("TeacherDivisionNotFound");

            return new ResponseDTO<TeacherDivisionPostDTO>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = new TeacherDivisionPostDTO()
                {
                    Id = teacherDivision.Id,
                    TeacherId = teacherDivision.TeacherId,
                    DivisionId = teacherDivision.DivisionId,
                    TeacherStatus = teacherDivision.TeacherStatus,
                    StartDate = teacherDivision.StartDate,
                    EndDate = teacherDivision.EndDate,
                    Observations = teacherDivision.Observations
                }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al intentar asignar al docente en la división: " + e.Message);
            
            if (e.Message == "TeacherDivisionNotFound")
                return new ResponseDTO<TeacherDivisionPostDTO>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "¡El docente asignado a una división que se desea leer no existe!",
                    Object = null
                };
            
            return new ResponseDTO<TeacherDivisionPostDTO>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar asignar al docente en la división!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<TeacherDivisionObservationDTO>> GetObservationByTeacherDivisionId(long id)
    {
        try
        {
            var td = await bbdd.TeacherDivisions.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (td == null) throw new Exception($"No hay un docente asignado con ese ID.");

            return new ResponseDTO<TeacherDivisionObservationDTO>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = new TeacherDivisionObservationDTO()
                {
                    TeacherDivisionId= td.Id,
                    Observation = td.Observations
                }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al intentar obtener la observación de un docente asignado: " + e.Message);
            return new ResponseDTO<TeacherDivisionObservationDTO>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"¡Hubo un error al intentar obtener la observación del docente asignado!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> AsignTeacherIntoDivision(TeacherDivisionPostDTO teacherDivisionPostDTO)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var teacher = await bbdd.People.FirstOrDefaultAsync(t => t.Id == teacherDivisionPostDTO.TeacherId);
            var division = await bbdd.DivisionTemplates.FirstOrDefaultAsync(t => t.Id == teacherDivisionPostDTO.DivisionId);
            
            if(teacher == null || division == null) throw new Exception("DivisionOrTeacherNotFound");

            bbdd.TeacherDivisions.Add(new TeacherDivision()
            {
                TeacherId = teacher.Id,
                DivisionId = division.Id,
                TeacherStatus = teacherDivisionPostDTO.TeacherStatus,
                StartDate = teacherDivisionPostDTO.StartDate,
                EndDate = teacherDivisionPostDTO.EndDate,
                Observations = teacherDivisionPostDTO.Observations,
            });

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Docente asignado a la división con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error al intentar asignar al docente en la división: " + e.Message);

            if (e.Message == "DivisionOrTeacherNotFound")
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "¡El docente o división que está intentando asignar no existe!",
                    Object = null
                };
            
            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar asignar al docente en la división!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> UpdateAsignationInDivision(TeacherDivisionPostDTO teacherDivisionPutDTO)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var teacherDivision = await bbdd.TeacherDivisions.FirstOrDefaultAsync(t => t.Id == teacherDivisionPutDTO.Id);
            if(teacherDivision == null) throw new Exception("TeacherDivisionNotFound");

            if (teacherDivision.TeacherId != teacherDivisionPutDTO.TeacherId)
            {
                var teacher = await bbdd.People.FirstOrDefaultAsync(t => t.Id == teacherDivisionPutDTO.TeacherId);
                if(teacher == null) throw new Exception("TeacherNotFound");
                teacherDivision.TeacherId = teacherDivisionPutDTO.TeacherId;
            }
            
            teacherDivision.TeacherStatus = teacherDivisionPutDTO.TeacherStatus;
            teacherDivision.StartDate = teacherDivisionPutDTO.StartDate;
            teacherDivision.EndDate = teacherDivisionPutDTO.EndDate;
            teacherDivision.Observations = teacherDivisionPutDTO.Observations;
            
            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Docente asignado a la división actualizado con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error al intentar actualizar asignación del docente en la división: " + e.Message);
            
            if (e.Message == "TeacherDivisionNotFound")
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "¡El docente asignado a una división que desea actualizar no existe!",
                    Object = null
                };
            
            if (e.Message == "TeacherNotFound")
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "¡El docente que está intentando asignar no existe!",
                    Object = null
                };
            
            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar actualizar la asignación del docente en la división!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> EditObservation(TeacherDivisionObservationDTO observationDTO)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            Console.WriteLine($"ID DIVTEACH: {observationDTO.TeacherDivisionId}");
            var td = await bbdd.TeacherDivisions.FindAsync(observationDTO.TeacherDivisionId);
            if (td == null)
                throw new Exception(
                    "La observación del docente asignado que se desea actualizar no se puede porque no existe.");

            td.UpdatedBy = observationDTO.UpdatedById;
            td.Observations = observationDTO.Observation;

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Observación actualizada con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error al intentar actualizar la observación del docente asignado: " + e.Message);
            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar actualizar la observación!",
                Object = null
            };
        }
    }
}