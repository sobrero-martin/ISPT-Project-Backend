using BD;
using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.ExamDTO;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Exams;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Repositorio.Repository.Exams
{
    public class FinalExamRepository : IFinalExamRepository
    {
        private readonly AppDbContext context;

        public FinalExamRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<List<FinalExamDTO>>> GetFull()
        {
            try
            {


                var exams = await context.Set<FinalExam>()
                    .AsNoTracking()
                    .Select(e => new FinalExamDTO
                    {
                        Id = e.Id,
                        SubjectName = e.Subject!.Name,
                        Date = e.Date,
                        Time = e.Time,
                        RecordBook = e.RecordBook,
                        PageNumber = e.PageNumber
                    })
                    .ToListAsync();

                return new ResponseDTO<List<FinalExamDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = exams,
                    Message = "Listado de mesas de exámen obtenido exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el listado de mesas de exámen: {ex.Message}");
                return new ResponseDTO<List<FinalExamDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Error al obtener el listado de mesas de exámen."
                };
            }
        }

        public async Task<ResponseDTO<string>> Post(FinalExamPostDTO exam)
        {
            try
            {
                var finalExamEntity = new FinalExam
                {
                    Id = exam.Id,
                    CreatedBy = exam.CreatedById ?? Guid.Empty,
                    SubjectId = exam.SubjectId,
                    PersonId = exam.PersonId,
                    Date = exam.Date,
                    Time = exam.Time,
                    RecordBook = exam.RecordBook,
                    PageNumber = exam.PageNumber
                };

                await context.Set<FinalExam>().AddAsync(finalExamEntity);
                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "Operación exitosa.",
                    Object = $"¡Mesa de examen:{finalExamEntity.Id} creada con éxito!"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la mesa de examen: {ex.Message}");

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al crear la mesa de examen.",
                    Object = null
                };
            }
        }
    }
}
