using BD;
using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.ExamDTO;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Exams;
using System.Net;

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
                    .Where(fe => fe.state)
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
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var finalExamEntity = new FinalExam
                {
                    Id = exam.Id,
                    CreatedBy = exam.CreatedById ?? Guid.Empty,
                    SubjectId = exam.SubjectId,
                    Date = exam.Date,
                    Time = exam.Time,
                    RecordBook = exam.RecordBook,
                    PageNumber = exam.PageNumber
                };

                await context.Set<FinalExam>().AddAsync(finalExamEntity);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

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
                await transaction.RollbackAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al crear la mesa de examen.",
                    Object = null
                };
            }
        }

        public Task<ResponseDTO<string>> Edit(FinalExamPostDTO exam)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO<string>> Delete(long id)
        {
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var me = await context.Set<FinalExam>().FirstOrDefaultAsync(f => f.Id == id);
                if (me == null) throw new Exception("FinalExamNotFound");

                me.state = false;
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "¡Operación éxitosa!",
                    Object = "¡Mesa de examen borrada con éxito!"
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al eliminar la mesa de examen: {e.Message}");
                await transaction.RollbackAsync();

                if (e.Message.Contains("FinalExamNotFound"))
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "La mesa de examen que se desea eliminar no existe.",
                        Object = null
                    };

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al eliminar la mesa de examen.",
                    Object = null
                };
            }
        }
    }
}