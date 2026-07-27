using BD;
using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Careers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Repositorio.Repository.Careers
{
    public class DivisionTemplateRepository : IDivisionTemplateRepository
    {
        private readonly AppDbContext context;

        public DivisionTemplateRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<List<DivisionTemplateDTO>>> GetBySubject(long subjectId)
        {
            try
            {
                var subject = await context.Set<Subject>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == subjectId);

                if (subject == null)
                {
                    return new ResponseDTO<List<DivisionTemplateDTO>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Espacio curricular no encontrado."
                    };
                }

                var divisionTemplates = await context.Set<DivisionTemplate>()
                    .AsNoTracking()
                    .Where(s => s.SubjectId == subjectId)
                    .Select(s => new DivisionTemplateDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        State = s.TemplateState
                    })
                    .ToListAsync();

                return new ResponseDTO<List<DivisionTemplateDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = divisionTemplates,
                    Message = "Plantillas de división obtenidas exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener divisiones por espacio curricular: {ex.Message}");

                return new ResponseDTO<List<DivisionTemplateDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener las plantillas de división."
                };
            }
        }

        public async Task<ResponseDTO<string>> Post(long subjectId, Guid? CreatedById)
        {
            try
            {
                var subjectExists = await context.Set<Subject>()
                    .AnyAsync(s => s.Id == subjectId);

                if (!subjectExists)
                {
                    throw new Exception("SubjectNotFound");
                }

                var lastTemplate = await context.Set<DivisionTemplate>()
                    .Where(d => d.SubjectId == subjectId)
                    .OrderByDescending(d => d.Name)
                    .FirstOrDefaultAsync();

                string nextName = lastTemplate == null
                    ? "A"
                    : ((char)(lastTemplate.Name[0] + 1)).ToString();

                var divisionTemplate = new DivisionTemplate
                {
                    SubjectId = subjectId,
                    Name = nextName,
                    TemplateState = true,
                    CreatedBy = CreatedById ?? Guid.Empty
                };

                context.Set<DivisionTemplate>().Add(divisionTemplate);
                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.Created,
                    Object = $"Plantilla de división {divisionTemplate.Name} creada exitosamente.",
                    Message = "Operación exitosa."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear plantilla de división: {ex.Message}");

                if (ex.Message == "SubjectNotFound")
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Espacio curricular no encontrado."
                    };
                }

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al crear la plantilla de división."
                };
            }
        }

        public async Task<ResponseDTO<string>> ChangeStatus(long divisionTemplateId)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                Console.WriteLine($"Division template: {divisionTemplateId}");
                var divisionTemplate = await context.DivisionTemplates.FirstOrDefaultAsync(d => d.Id == divisionTemplateId);

                if (divisionTemplate == null) throw new Exception("SubjectNotFound");
                
                divisionTemplate.TemplateState = !divisionTemplate.TemplateState;
                await context.SaveChangesAsync();
                
                await transaction.CommitAsync();
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Éxito",
                    Object = "¡Estado de división cambiado con éxito!"
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Error al intentar cambiar el estado de una división: " + e.Message);

                if (e.Message == "SubjectNotFound")
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Object = null,
                        Message = "División no encontrada."
                    };
                }
                
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "¡Hubo un error al intentar cambiar el estado de la división!",
                    Object = null
                };
            }
        }
    }
}
