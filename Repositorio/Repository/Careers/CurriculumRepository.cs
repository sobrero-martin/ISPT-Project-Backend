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
    public class CurriculumRepository : ICurriculumRepository
    {
        private readonly AppDbContext context;

        public CurriculumRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<List<CurriculumDTO>>> GetByCareer(long careerId)
        {
            try
            {
                var career = await context.Set<Career>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == careerId);

                if (career == null)
                {
                    return new ResponseDTO<List<CurriculumDTO>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Carrera no encontrada"
                    };
                }

                var curriculum = await context.Set<Curriculum>()
                    .AsNoTracking()
                    .Where(c => c.CareerId == careerId)
                    .Select(c => new CurriculumDTO
                    {
                        Id = c.Id,
                        Resolution = c.Resolution,
                        Duration = c.Duration,
                        VigencyDate = c.VigencyDate,
                        EndDate = c.EndDate
                    })
                    .ToListAsync();

                return new ResponseDTO<List<CurriculumDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = curriculum,
                    Message = "Planes de estudio obtenidos exitosamente",
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los planes de estudio para la carrera con ID {careerId}: {ex.Message}");

                return new ResponseDTO<List<CurriculumDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = $"Ocurrió un error al obtener los planes de estudio"
                };


            }
        }

        public async Task<ResponseDTO<CurriculumDTO>> GetById (long id)
        {
            try
            {
                var curriculum = await context.Set<Curriculum>()
                    .AsNoTracking()
                    .Where(c => c.Id == id)
                    .Select(c => new CurriculumDTO
                    {
                        Id = c.Id,
                        Resolution = c.Resolution,
                        Duration = c.Duration,
                        VigencyDate = c.VigencyDate,
                        EndDate = c.EndDate
                    })
                    .FirstOrDefaultAsync();

                if (curriculum == null)
                {
                    return new ResponseDTO<CurriculumDTO>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Plan de estudio no encontrado"
                    };
                }

                return new ResponseDTO<CurriculumDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Plan de estudio obtenido exitosamente",
                    Object = curriculum
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el plan de estudio: {ex.Message}");
                return new ResponseDTO<CurriculumDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener el plan de estudio"
                };
            }

        }

        public async Task<ResponseDTO<string>> Post(CurriculumPostDTO curriculumPostDTO)
        {
            try
            {
                var curriculumEntity = new Curriculum
                {
                    Resolution = curriculumPostDTO.Resolution,
                    Duration = curriculumPostDTO.Duration,
                    VigencyDate = curriculumPostDTO.VigencyDate,
                    EndDate = curriculumPostDTO.EndDate,
                    CreatedBy = curriculumPostDTO.CreatedById ?? Guid.Empty,
                    CareerId = curriculumPostDTO.CareerId
                };
                await context.Set<Curriculum>().AddAsync(curriculumEntity);
                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.Created,
                    Object = $"Plan de estudio {curriculumEntity.Resolution} creado exitosamente",
                    Message = "Operación exitosa."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el plan de estudio: {ex.Message}");
                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al crear el plan de estudio"
                };
            }
        }

        public async Task<ResponseDTO<string>> Put(long id, CurriculumPostDTO curriculumPostDTO)
        {
            try
            {

                if (id != curriculumPostDTO.Id)
                {
                    throw new Exception("idMismatch");
                }

                var existingCurriculum = await context.Set<Curriculum>().FirstOrDefaultAsync(c => c.Id == id);

                if (existingCurriculum == null)
                {
                    throw new Exception("curriculumNotFound");
                }

                existingCurriculum.Resolution = curriculumPostDTO.Resolution;
                existingCurriculum.Duration = curriculumPostDTO.Duration;
                existingCurriculum.VigencyDate = curriculumPostDTO.VigencyDate;
                existingCurriculum.EndDate = curriculumPostDTO.EndDate;
                existingCurriculum.UpdatedBy = curriculumPostDTO.UpdatedById ?? Guid.Empty;

                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = $"Plan de estudio {existingCurriculum.Resolution} actualizado exitosamente",
                    Message = "Operación exitosa."
                };
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"Error al actualizar el plan de estudio: {ex.Message}");

                if(ex.Message == "idMismatch")
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Object = null,
                        Message = "El ID del plan de estudio no coincide con el ID proporcionado"
                    };
                }

                if(ex.Message == "curriculumNotFound")
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Plan de estudio no encontrado"
                    };
                }

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al actualizar el plan de estudio"
                };
            }

        }
    }

}
