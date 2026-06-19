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
                        StartDate = c.StartDate,
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

                /*
                var careers = await context.Set<Curriculum>().Where(c => c.CareerId == careerId).ToListAsync();
            var curriculumDTOs = new List<CurriculumDTO>();
            foreach (var career in careers)
            {
                curriculumDTOs.Add(new CurriculumDTO
                {
                    Id = career.Id,
                    Resolution = career.Resolution,
                    Duration = career.Duration,
                    StartDate = career.StartDate,
                    VigencyDate = career.VigencyDate,
                    EndDate = career.EndDate
                });
            }
                return curriculumDTOs;*/

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
                        StartDate = c.StartDate,
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

            /*
            var curriculum = await context.Set<Curriculum>().FirstOrDefaultAsync(x => x.Id == id);

            if (curriculum == null) return null;

            return new CurriculumDTO
            {
                Id = curriculum.Id,
                Resolution = curriculum.Resolution,
                Duration = curriculum.Duration,
                StartDate = curriculum.StartDate,
                VigencyDate = curriculum.VigencyDate,
                EndDate = curriculum.EndDate
            };*/
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
                        StartDate = curriculum.StartDate,
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
            /*
            try
            {
                await context.Set<Curriculum>().AddAsync(curriculum);
                await context.SaveChangesAsync();
                CurriculumDTO curriculumDTO = new CurriculumDTO
                {
                    Id = curriculum.Id,
                    Resolution = curriculum.Resolution,
                    Duration = curriculum.Duration,
                    StartDate = curriculum.StartDate,
                    VigencyDate = curriculum.VigencyDate,
                    EndDate = curriculum.EndDate
                };
                return curriculumDTO;
            }
            catch (Exception)
            {
                throw;
            }*/
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
                existingCurriculum.StartDate = curriculum.StartDate;
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
            /*
            if (id != curriculum.Id) return false;

            var existing = await context.Set<Curriculum>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null) return false;

            existing.Resolution = curriculum.Resolution;
            existing.Duration = curriculum.Duration;
            existing.StartDate = curriculum.StartDate;
            existing.EndDate = curriculum.EndDate;
            existing.VigencyDate = curriculum.VigencyDate;

            await context.SaveChangesAsync();
            return true;*/
        }
    }

}
