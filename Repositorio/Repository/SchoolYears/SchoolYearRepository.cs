using BD;
using BD.Entidades;
using BD.Entities;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.SchoolYears;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Repositorio.Repository.SchoolYears
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
                var schoolYearsData = await context.Set<SchoolYear>()
                    .AsNoTracking()
                    .Include(s => s.Curriculum)
                        .ThenInclude(c => c.Career)
                    .Select(s => new
                    {
                        s.Id,
                        CareerName = s.Curriculum.Career.Name,
                        Resolution = s.Curriculum.Resolution,
                        s.SchoolYearNumber,
                        s.CurriculumId,
                        s.YearNumber 
                    })
                    .ToListAsync();

                var groupedSchoolYears = schoolYearsData
                    .GroupBy(s => new { s.SchoolYearNumber, s.CurriculumId, s.CareerName, s.Resolution })
                    .Select(g => new SchoolYearDTO
                    {
                        Id = g.First().Id,
                        CareerName = g.Key.CareerName,
                        Resolution = g.Key.Resolution,
                        SchoolYearNumber = g.Key.SchoolYearNumber,

                        YearsFormatted = string.Join(", ", g.Select(x => $"{x.YearNumber}°").OrderBy(y => y))
                    })
                    .ToList();

                return new ResponseDTO<List<SchoolYearDTO>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Listado de ciclos lectivos obtenido exitosamente.",
                    Object = groupedSchoolYears
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
                if (schoolYear.StartDate >= schoolYear.EndDate)
                {
                    return new ResponseDTO<SchoolYearPostDTO>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "La fecha de inicio debe ser anterior a la fecha de fin.",
                        Object = null
                    };
                }

                var existingYears = await context.Set<SchoolYear>()
                    .Where(sy => sy.CurriculumId == schoolYear.CurriculumId &&
                                 sy.SchoolYearNumber == schoolYear.SchoolYearNumber &&
                                 schoolYear.Years.Contains(sy.YearNumber))
                    .Select(sy => sy.YearNumber)
                    .ToListAsync();

                if (existingYears.Any())
                {
                    var yearsFormatted = string.Join(", ", existingYears);
                    return new ResponseDTO<SchoolYearPostDTO>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = $"Ya existen registros creados para el año lectivo {schoolYear.SchoolYearNumber} en los siguientes años de cursado: {yearsFormatted}.",
                        Object = null
                    };
                }

                foreach (var yearNum in schoolYear.Years)
                {
                    var schoolYearEntity = new SchoolYear
                    {
                        CurriculumId = schoolYear.CurriculumId,
                        SchoolYearNumber = schoolYear.SchoolYearNumber,
                        YearNumber = yearNum,
                        CreatedBy = schoolYear.CreatedById ?? Guid.Empty,
                        StartDate = schoolYear.StartDate, 
                        EndDate = schoolYear.EndDate       
                    };

                    await context.Set<SchoolYear>().AddAsync(schoolYearEntity);
                    await context.SaveChangesAsync();

                    var subjectIds = await context.Set<Subject>()
                        .Where(s => s.CurriculumId == schoolYearEntity.CurriculumId && s.Year == yearNum)
                        .Select(s => s.Id)
                        .ToListAsync();

                    if (!subjectIds.Any()) continue;

                    var divisionTemplates = await context.Set<DivisionTemplate>()
                        .Where(d => subjectIds.Contains(d.SubjectId))
                        .ToListAsync();

                    if (!divisionTemplates.Any()) continue;

                    var divisions = divisionTemplates.Select(dt => new Division
                    {
                        DivisionTemplateId = dt.Id,
                        SchoolYearId = schoolYearEntity.Id,
                        DivisionState = "Active",
                        CreatedBy = schoolYear.CreatedById ?? Guid.Empty,
                    }).ToList();

                    await context.Set<Division>().AddRangeAsync(divisions);
                    await context.SaveChangesAsync();

                    var scheduleTemplates = await context.Set<ScheduleTemplate>()
                        .Where(s => divisionTemplates.Select(dt => dt.Id).Contains(s.DivisionTemplateId))
                        .ToListAsync();

                    if (scheduleTemplates.Any())
                    {
                        var schedules = scheduleTemplates.Select(st => new Schedule
                        {
                            DivisionId = divisions.First(d => d.DivisionTemplateId == st.DivisionTemplateId).Id,
                            Day = st.Day,
                            StartTime = st.StartTime,
                            EndTime = st.EndTime,
                            CreatedBy = schoolYear.CreatedById ?? Guid.Empty,
                        }).ToList();

                        await context.Set<Schedule>().AddRangeAsync(schedules);
                        await context.SaveChangesAsync();
                    }
                }

                return new ResponseDTO<SchoolYearPostDTO>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "Ciclos lectivos creados exitosamente.",
                    Object = schoolYear
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
