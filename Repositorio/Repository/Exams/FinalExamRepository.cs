using BD;
using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.ExamDTO;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Exams;
using System.Net;
using BD.Entities;
using DTO.ENUM;

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

        public async Task<ResponseDTO<FinalExamPostDTO>> GetFinalExamById(long id)
        {
            try
            {
                var exam = await context.Set<FinalExam>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (exam == null)
                {
                    return new ResponseDTO<FinalExamPostDTO>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "No se encontró la mesa de examen solicitada.",
                        Object = null
                    };
                }
                
                var tribunals = await context.FinalExamTribunals
                    .AsNoTracking()
                    .Where(t => t.FinalExamId == id)
                    .ToListAsync();
                
                var president =
                    tribunals.FirstOrDefault(t => t.FinalExamTribunalRole == EnumFinalExamTribunalRole.Titular);
                
                var vocals = tribunals.Where(t => t.FinalExamTribunalRole == EnumFinalExamTribunalRole.Vocal).ToList();
                
                var examDto = new FinalExamPostDTO
                {
                    Id = exam.Id,
                    CreatedById = exam.CreatedBy,
                    SubjectId = exam.SubjectId,
                    Date = exam.Date,
                    Time = exam.Time,
                    RecordBook = exam.RecordBook,
                    PageNumber = exam.PageNumber,
                    Tribunal = new TribunalExamDTO
                    {
                        PresidentId = president?.PersonId ?? 0,
                        Vocal1Id = vocals.Count > 0 ? vocals[0].PersonId : null,
                        Vocal2Id = vocals.Count > 1 ? vocals[1].PersonId : null
                    }
                };

                return new ResponseDTO<FinalExamPostDTO>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Operación exitosa.",
                    Object = examDto
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la mesa de examen con ID {id}: {ex.Message}");

                return new ResponseDTO<FinalExamPostDTO>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al consultar la mesa de examen.",
                    Object = null
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

                context.Set<FinalExam>().Add(finalExamEntity);
                await context.SaveChangesAsync();

                context.FinalExamTribunals.Add(new FinalExamTribunal()
                {
                    CreatedBy = exam.CreatedById ?? Guid.Empty,
                    FinalExamId = finalExamEntity.Id,
                    FinalExamTribunalRole = EnumFinalExamTribunalRole.Titular,
                    PersonId = exam.Tribunal.PresidentId
                });

                if (exam.Tribunal.Vocal1Id is > 0)
                {
                    context.FinalExamTribunals.Add(new FinalExamTribunal()
                    {
                        CreatedBy = exam.CreatedById ?? Guid.Empty,
                        FinalExamId = finalExamEntity.Id,
                        FinalExamTribunalRole = EnumFinalExamTribunalRole.Vocal,
                        PersonId = (long)exam.Tribunal.Vocal1Id
                    });
                }

                if (exam.Tribunal.Vocal2Id is > 0)
                {
                    context.FinalExamTribunals.Add(new FinalExamTribunal()
                    {
                        CreatedBy = exam.CreatedById ?? Guid.Empty,
                        FinalExamId = finalExamEntity.Id,
                        FinalExamTribunalRole = EnumFinalExamTribunalRole.Vocal,
                        PersonId = (long)exam.Tribunal.Vocal2Id
                    });
                }

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

        public async Task<ResponseDTO<string>> Edit(FinalExamPostDTO exam)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingExam = await context.Set<FinalExam>().FindAsync(exam.Id);

                if (existingExam == null)
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "No se encontró la mesa de examen especificada."
                    };
                }

                var userGuid = exam.UpdatedById ?? exam.CreatedById ?? Guid.Empty;

                existingExam.SubjectId = exam.SubjectId;
                existingExam.Date = exam.Date;
                existingExam.Time = exam.Time;
                existingExam.RecordBook = exam.RecordBook;
                existingExam.PageNumber = exam.PageNumber;

                var currentTribunals = await context.FinalExamTribunals
                    .Where(t => t.FinalExamId == exam.Id)
                    .ToListAsync();

                context.FinalExamTribunals.RemoveRange(currentTribunals);

                var newTribunalList = new List<FinalExamTribunal>
                {
                    new()
                    {
                        CreatedBy = userGuid,
                        FinalExamId = exam.Id,
                        FinalExamTribunalRole = EnumFinalExamTribunalRole.Titular,
                        PersonId = exam.Tribunal.PresidentId
                    }
                };

                if (exam.Tribunal.Vocal1Id is > 0)
                {
                    newTribunalList.Add(new FinalExamTribunal
                    {
                        CreatedBy = userGuid,
                        FinalExamId = exam.Id,
                        FinalExamTribunalRole = EnumFinalExamTribunalRole.Vocal,
                        PersonId = exam.Tribunal.Vocal1Id.Value
                    });
                }

                if (exam.Tribunal.Vocal2Id is > 0)
                {
                    newTribunalList.Add(new FinalExamTribunal
                    {
                        CreatedBy = userGuid,
                        FinalExamId = exam.Id,
                        FinalExamTribunalRole = EnumFinalExamTribunalRole.Vocal,
                        PersonId = exam.Tribunal.Vocal2Id.Value
                    });
                }

                context.FinalExamTribunals.AddRange(newTribunalList);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Operación exitosa.",
                    Object = $"¡Mesa de examen #{exam.Id} actualizada con éxito!"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la mesa de examen: {ex.Message}");
                await transaction.RollbackAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al actualizar la mesa de examen.",
                    Object = null
                };
            }
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