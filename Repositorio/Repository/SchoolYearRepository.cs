using BD;
using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Repositorio.Repository
{
    public class SchoolYearRepository : ISchoolYearRepository
    {
        private readonly AppDbContext context;

        public SchoolYearRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<List<SchoolYearDTO>>> GetFull()
        {
            try
            {
                var schoolYears = await context.Set<SchoolYear>()
                    .AsNoTracking()
                    .Include(s => s.Curriculum)
                        .ThenInclude(c => c.Career)
                    .Select(s => new SchoolYearDTO
                    {
                        Id = s.Id,
                        CareerName = s.Curriculum.Career.Name,
                        Resolution = s.Curriculum.Resolution,
                        SchoolYearNumber = s.SchoolYearNumber
                    })
                    .ToListAsync();

                return new ResponseDTO<List<SchoolYearDTO>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Listado de ciclos lectivos obtenido exitosamente.",
                    Object = schoolYears
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el listado: {ex.Message}");

                return new ResponseDTO<List<SchoolYearDTO>>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al obtener el listado.",
                    Object = null
                };
            }
        }

        public async Task<ResponseDTO<List<SchoolYearPostDTO>>> GetRaw()
        {
            try
            {
                var schoolYears = await context.Set<SchoolYear>()
                    .AsNoTracking()
                    .Select(s => new SchoolYearPostDTO
                    {
                        Id = s.Id,
                        CurriculumId = s.CurriculumId,
                        SchoolYearNumber = s.SchoolYearNumber
                    })
                    .ToListAsync();

                return new ResponseDTO<List<SchoolYearPostDTO>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Listado de ciclos lectivos obtenido exitosamente.",
                    Object = schoolYears
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el listado de ciclos lectivos: {ex.Message}");

                return new ResponseDTO<List<SchoolYearPostDTO>>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al obtener el listado de ciclos lectivos.",
                    Object = null
                };
            }
        }

        public async Task<ResponseDTO<SchoolYearPostDTO>> GetById(long id)
        {
            try
            {
                var schoolYear = await context.Set<SchoolYear>()
                    .AsNoTracking()
                    .Where(s => s.Id == id)
                    .Select(s => new SchoolYearPostDTO
                    {
                        Id = s.Id,
                        CurriculumId = s.CurriculumId,
                        SchoolYearNumber = s.SchoolYearNumber
                    })
                    .FirstOrDefaultAsync();

                if (schoolYear == null)
                {
                    return new ResponseDTO<SchoolYearPostDTO>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "Ciclo lectivo no encontrado.",
                        Object = null
                    };
                }

                return new ResponseDTO<SchoolYearPostDTO>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Ciclo lectivo obtenido exitosamente.",
                    Object = schoolYear
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el ciclo lectivo: {ex.Message}");

                return new ResponseDTO<SchoolYearPostDTO>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al obtener el ciclo lectivo.",
                    Object = null
                };
            }
        }

        public async Task<ResponseDTO<SchoolYearPostDTO>> Post(SchoolYearPostDTO schoolYear)
        {
            try
            {
                var schoolYearEntity = new SchoolYear
                {
                    Id = schoolYear.Id,
                    CurriculumId = schoolYear.CurriculumId,
                    SchoolYearNumber = schoolYear.SchoolYearNumber
                };

                await context.Set<SchoolYear>().AddAsync(schoolYearEntity);
                await context.SaveChangesAsync();

                return new ResponseDTO<SchoolYearPostDTO>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "Ciclo lectivo creado exitosamente.",
                    Object = new SchoolYearPostDTO
                    {
                        Id = schoolYear.Id,
                        CurriculumId = schoolYear.CurriculumId,
                        SchoolYearNumber = schoolYear.SchoolYearNumber
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el ciclo lectivo: {ex.Message}");

                return new ResponseDTO<SchoolYearPostDTO>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al crear el ciclo lectivo.",
                    Object = null
                };
            }
        }
    }
}
