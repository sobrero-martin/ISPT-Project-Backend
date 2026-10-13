using BD;
using BD.Entidades;
using BD.Entities;
using DTO.DTOs.CoursesDTO;
using DTO.DTOs.DTO_Response;
using DTO.ENUM;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Courses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Repositorio.Repository.Courses
{
    public class DivisionExamRepository : IDivisionExamRepository
    {
        private readonly AppDbContext bbdd;

        public DivisionExamRepository(AppDbContext bbdd)
        {
            this.bbdd = bbdd;
        }

        public async Task<ResponseDTO<List<DivisionExamReadDTO>>> GetAllEvaluations(long divisionId)
        {
            try
            {
                return new ResponseDTO<List<DivisionExamReadDTO>>()
                {
                    Message = "¡Operación exitosa!",
                    Object = await bbdd.DivisionExams.Where(de => de.DivisionId == divisionId && de.state).Select(r => new DivisionExamReadDTO
                    {
                        Id = r.Id,
                        Date = r.Date,
                        Number = r.Number,
                        Type = r.Type.ToString(),
                    }).AsNoTracking().ToListAsync(),
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener instancias evaluativas: {ex.Message}");

                return new ResponseDTO<List<DivisionExamReadDTO>>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al obtener las instancias evaluativas.",
                    Object = null
                };
            }
        }

        public async Task<ResponseDTO<string>> CreateEvaluation(DivisionExamPostDTO divisionExamDTO)
        {
            using var transaction = await bbdd.Database.BeginTransactionAsync();
            try
            {
                var divisionExam = new DivisionExam
                {
                    CreatedBy = divisionExamDTO.CreatedById ?? Guid.Empty,
                    DivisionId = divisionExamDTO.DivisionId,
                    Date = divisionExamDTO.Date,
                    Type = divisionExamDTO.Type,
                    Number = divisionExamDTO.Number,
                    state = true
                };

                bbdd.DivisionExams.Add(divisionExam);
                await bbdd.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "¡Operación exitosa!",
                    Object = $"¡Instancia evaluativa creada con éxito!"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la instancia evaluativa: {ex.Message}");
                await transaction.RollbackAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al crear la instancia evaluativa.",
                    Object = null
                };
            }
        }

        public async Task<ResponseDTO<string>> DeleteEvaluation(long evaluationId)
        {
            using var transaction = await bbdd.Database.BeginTransactionAsync();
            try
            {
                var divisionExam = await bbdd.DivisionExams.FirstOrDefaultAsync(de => de.Id == evaluationId);
                if (divisionExam == null) throw new Exception("NotFound");

                divisionExam.state = false;

                await bbdd.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "¡Operación exitosa!",
                    Object = $"¡Instancia evaluativa eliminada con éxito!"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la instancia evaluativa: {ex.Message}");
                await transaction.RollbackAsync();

                if (ex.Message.Contains("NotFound"))
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "La instancia evaluativa que se desea eliminar no existe.",
                        Object = null
                    };

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al intentar eliminar la instancia evaluativa.",
                    Object = null
                };
            }
        }

    }
}
