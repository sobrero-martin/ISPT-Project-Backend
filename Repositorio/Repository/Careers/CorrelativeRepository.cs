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
    public class CorrelativeRepository : ICorrelativeRepository
    {
        private readonly AppDbContext context;

        public CorrelativeRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<bool>> SaveChanges(long subjectId, List<CorrelativeChangeDTO> changes)
        {
            try
            {
                foreach (var change in changes)
                {
                    var correlativa = await context.Set<Correlative>()
                        .FirstOrDefaultAsync(c => c.SubjectId == subjectId && c.SubjectCorrelativeId == change.SubjectCorrelativeId);

                    if (change.IsCorrelative && correlativa == null)
                    {
                        await context.Set<Correlative>().AddAsync(new Correlative
                        {
                            SubjectId = subjectId,
                            SubjectCorrelativeId = change.SubjectCorrelativeId,
                            CreatedBy = change.CreatedById ?? Guid.Empty
                        });
                    }
                    else if (!change.IsCorrelative && correlativa != null)
                    {
                        context.Set<Correlative>().Remove(correlativa);
                    }
                }
                await context.SaveChangesAsync();

                return new ResponseDTO<bool>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = true,
                    Message = "Cambios en correlativas guardados exitosamente"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar los cambios en correlativas: {ex.Message}");
                return new ResponseDTO<bool>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = false,
                    Message = "Ocurrió un error al guardar los cambios en correlativas"
                };
            }
        }

    }
}
