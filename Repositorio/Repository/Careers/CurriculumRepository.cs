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
                    Message = "Planes de estudio obtenidos exitosamente",
                    Object = curriculum
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

        public async Task<ResponseDTO<CurriculumDTO>> Post(Curriculum curriculum)
        {
            try
            {
                await context.Set<Curriculum>().AddAsync(curriculum);
                await context.SaveChangesAsync();

                return new ResponseDTO<CurriculumDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.Created,
                    Object = new CurriculumDTO
                    {
                        Id = curriculum.Id,
                        Resolution = curriculum.Resolution,
                        Duration = curriculum.Duration,
                        VigencyDate = curriculum.VigencyDate,
                        EndDate = curriculum.EndDate
                    },
                    Message = "Plan de estudio creado exitosamente"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el plan de estudio: {ex.Message}");
                return new ResponseDTO<CurriculumDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al crear el plan de estudio"
                };
            }
        }

        public async Task<ResponseDTO<string>> Put(long id, Curriculum curriculum)
        {
            try
            {
                if (id != curriculum.Id)
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Object = null,
                        Message = "El ID del plan de estudio no coincide con el ID proporcionado"
                    };
                }

                var existingCurriculum = await context.Set<Curriculum>().FirstOrDefaultAsync(c => c.Id == id);

                if (existingCurriculum == null)
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Plan de estudio no encontrado"
                    };
                }

                existingCurriculum.Resolution = curriculum.Resolution;
                existingCurriculum.Duration = curriculum.Duration;
                existingCurriculum.VigencyDate = curriculum.VigencyDate;
                existingCurriculum.EndDate = curriculum.EndDate;

                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = null,
                    Message = "Plan de estudio actualizado exitosamente"
                };
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"Error al actualizar el plan de estudio: {ex.Message}");

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
