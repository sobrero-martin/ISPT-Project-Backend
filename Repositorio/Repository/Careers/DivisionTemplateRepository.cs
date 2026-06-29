using BD;
using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Careers;
using System;
using System.Collections.Generic;
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
                        Message = "Materia no encontrada."
                    };
                }

                var divisionTemplates = await context.Set<DivisionTemplate>()
                    .AsNoTracking()
                    .Where(s => s.SubjectId == subjectId)
                    .Select(s => new DivisionTemplateDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
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
                Console.WriteLine($"Error al obtener divisiones por materia: {ex.Message}");

                return new ResponseDTO<List<DivisionTemplateDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener las plantillas de división."
                };
            }
        }

        public async Task<ResponseDTO<DivisionTemplateDTO>> Post(long subjectId)
        {
            try
            {
                var subjectExists = await context.Set<Subject>()
                    .AnyAsync(s => s.Id == subjectId);

                if (!subjectExists)
                {
                    return new ResponseDTO<DivisionTemplateDTO>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Materia no encontrada."
                    };
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
                    Name = nextName
                };

                context.Set<DivisionTemplate>().Add(divisionTemplate);
                await context.SaveChangesAsync();

                return new ResponseDTO<DivisionTemplateDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.Created,
                    Object = new DivisionTemplateDTO
                    {
                        Id = divisionTemplate.Id,
                        Name = divisionTemplate.Name
                    },
                    Message = "Plantilla de división creada exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear plantilla de división: {ex.Message}");

                return new ResponseDTO<DivisionTemplateDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al crear la plantilla de división."
                };
            }
        }
    }
}
