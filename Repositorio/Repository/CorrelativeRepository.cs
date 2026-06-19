using BD;
using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Repository
{
    public class CorrelativeRepository : ICorrelativeRepository
    {
        private readonly AppDbContext context;

        public CorrelativeRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<CorrelativeDTO>> Post(Correlative correlative)
        {
            try
            {
                await context.Set<Correlative>().AddAsync(correlative);
                await context.SaveChangesAsync();

                return new ResponseDTO<CorrelativeDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.Created,
                    Object = new CorrelativeDTO
                    {
                        Id = correlative.Id,
                        SubjectId = correlative.SubjectId,
                    },
                    Message = "Correlativa creada exitosamente"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la correlativa: {ex.Message}");

                return new ResponseDTO<CorrelativeDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al crear la correlativa"
                };
            }
            /*
            try
            {
                await context.Set<Correlative>().AddAsync(correlative);
                await context.SaveChangesAsync();
                CorrelativeDTO correlativeDTO = new CorrelativeDTO
                {
                    Id = correlative.Id,
                    SubjectId = correlative.SubjectId,
                };
                return correlativeDTO;
            }
            catch (Exception)
            {
                throw;
            }*/
        }

        public async Task<ResponseDTO<bool>> Delete(long subjectId, long correlativeId)
        {
            try
            {
                var correlative = await context.Set<Correlative>()
                    .FirstOrDefaultAsync(c =>
                        c.SubjectId == subjectId &&
                        c.SubjectCorrelativeId == correlativeId);

                if (correlative == null)
                {
                    return new ResponseDTO<bool>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = false,
                        Message = "Correlativa no encontrada"
                    };
                }

                context.Set<Correlative>().Remove(correlative);
                await context.SaveChangesAsync();

                return new ResponseDTO<bool>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = true,
                    Message = "Correlativa eliminada exitosamente"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la correlativa: {ex.Message}");

                return new ResponseDTO<bool>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = false,
                    Message = "Ocurrió un error al eliminar la correlativa"
                };
            }
            /*
             var correlative = await context.Set<Correlative>()
            .FirstOrDefaultAsync(c =>
             c.SubjectId == subjectId &&
             c.SubjectCorrelativeId == correlativeId);

            if (correlative == null)
                return false;

            context.Set<Correlative>().Remove(correlative);
            await context.SaveChangesAsync();

            return true;*/
        }

        public async Task<ResponseDTO<bool>> Exists(long subjectId1, long subjectId2)
        {
            try
            {
                var exists = await context.Set<Correlative>()
                               .AnyAsync(c =>
                                   c.SubjectId == subjectId1 &&
                                   c.SubjectCorrelativeId == subjectId2);

                return new ResponseDTO<bool>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = exists,
                    Message = "Verificación de existencia completada"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar la existencia de la correlativa: {ex.Message}");
                return new ResponseDTO<bool>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = false,
                    Message = "Ocurrió un error al verificar la existencia de la correlativa"
                };
            }
            //return await context.Set<Correlative>().AnyAsync(c => c.SubjectId == subjectId1 && c.SubjectCorrelativeId == subjectId2);
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
                            SubjectCorrelativeId = change.SubjectCorrelativeId
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
