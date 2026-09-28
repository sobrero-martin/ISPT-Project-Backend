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

        //REVISAR
        public async Task<ResponseDTO<List<SchoolYearDTO>>> GetFull()
        {
            try
            {
                var schoolYears = await context.Set<SchoolYear>()
                    .AsNoTracking()
                    .Include(sy => sy.SchoolYearCurriculums) 
                        .ThenInclude(syc => syc.Curriculum)
                            .ThenInclude(c => c.Career)
                    .Select(sy => new SchoolYearDTO
                    {
                        Id = sy.Id,
                        SchoolYearNumber = sy.SchoolYearNumber,
                        
                        CareerName = sy.SchoolYearCurriculums
                            .Select(syc => syc.Curriculum.Career.Name)
                            .FirstOrDefault(),
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

        //REVISAR
        public async Task<ResponseDTO<List<SchoolYearByGradePostDTO>>> GetRaw()
        {
            try
            {
                var schoolYears = await context.Set<SchoolYear>()
                    .AsNoTracking()
                    .GroupJoin(
                        context.Set<SchoolYearCurriculum>(),
                        sy => sy.Id,
                        syc => syc.SchoolYearId,
                        (sy, sycs) => new { SchoolYear = sy, Curriculums = sycs.ToList() }
                    )
                    .ToListAsync();

                var resultList = new List<SchoolYearByGradePostDTO>();

                foreach (var item in schoolYears)
                {
                    var sy = item.SchoolYear;
                    var curriculums = item.Curriculums;

                    resultList.Add(new SchoolYearByGradePostDTO
                    {
                        Id = sy.Id,
                        SchoolYearNumber = sy.SchoolYearNumber,
                        CurriculumYear1 = curriculums.FirstOrDefault(c => c.SchoolYearNumber == 1)?.CurriculumId ?? 0,
                        CurriculumYear2 = curriculums.FirstOrDefault(c => c.SchoolYearNumber == 2)?.CurriculumId ?? 0,
                        CurriculumYear3 = curriculums.FirstOrDefault(c => c.SchoolYearNumber == 3)?.CurriculumId ?? 0,
                        CreatedById = sy.CreatedBy
                    });
                }

                return new ResponseDTO<List<SchoolYearByGradePostDTO>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Listado de ciclos lectivos obtenido exitosamente.",
                    Object = resultList
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el listado de ciclos lectivos: {ex.Message}");

                return new ResponseDTO<List<SchoolYearByGradePostDTO>>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al obtener el listado de ciclos lectivos.",
                    Object = null
                };
            }
        }

        //REVISAR
        public async Task<ResponseDTO<SchoolYearByGradePostDTO>> GetById(long id)
        {
            try
            {
                
                var schoolYear = await context.Set<SchoolYear>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (schoolYear == null)
                {
                    return new ResponseDTO<SchoolYearByGradePostDTO>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "Ciclo lectivo no encontrado.",
                        Object = null
                    };
                }

               
                var curriculums = await context.Set<SchoolYearCurriculum>()
                    .AsNoTracking()
                    .Where(syc => syc.SchoolYearId == id)
                    .ToListAsync();

               
                var schoolYearDto = new SchoolYearByGradePostDTO
                {
                    Id = schoolYear.Id,
                    SchoolYearNumber = schoolYear.SchoolYearNumber,
                    CurriculumYear1 = curriculums.FirstOrDefault(c => c.SchoolYearNumber == 1)?.CurriculumId ?? 0,
                    CurriculumYear2 = curriculums.FirstOrDefault(c => c.SchoolYearNumber == 2)?.CurriculumId ?? 0,
                    CurriculumYear3 = curriculums.FirstOrDefault(c => c.SchoolYearNumber == 3)?.CurriculumId ?? 0,
                    CreatedById = schoolYear.CreatedBy
                };

                return new ResponseDTO<SchoolYearByGradePostDTO>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Ciclo lectivo obtenido exitosamente.",
                    Object = schoolYearDto
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el ciclo lectivo: {ex.Message}");

                return new ResponseDTO<SchoolYearByGradePostDTO>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al obtener el ciclo lectivo.",
                    Object = null
                };
            }
        }
        /*
        public async Task<ResponseDTO<SchoolYearPostDTO>> Post(SchoolYearPostDTO schoolYear)
        {
            try
            {
                var schoolYearEntity = new SchoolYear
                {
                    Id = schoolYear.Id,
                    CurriculumId = schoolYear.CurriculumId,
                    SchoolYearNumber = schoolYear.SchoolYearNumber,
                    CreatedBy = schoolYear.CreatedById ?? Guid.Empty,
                };

                await context.Set<SchoolYear>().AddAsync(schoolYearEntity);
                await context.SaveChangesAsync();

                var subjectIds = await context.Set<Subject>()
                    .Where(s => s.CurriculumId == schoolYearEntity.CurriculumId)
                    .Select(s => s.Id)
                    .ToListAsync();

                var divisionTemplates = await context.Set<DivisionTemplate>()
                    .Where(d => subjectIds.Contains(d.SubjectId))
                    .ToListAsync();

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

                return new ResponseDTO<SchoolYearPostDTO>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "Ciclo lectivo creado exitosamente.",
                    Object = new SchoolYearPostDTO
                    {
                        Id = schoolYear.Id,
                        CurriculumId = schoolYear.CurriculumId,
                        SchoolYearNumber = schoolYear.SchoolYearNumber,
                        CreatedById = schoolYear.CreatedById ?? Guid.Empty,
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
        */

        public async Task<ResponseDTO<List<SchoolYearCurriculumDTO>>> GetCurriculumsBySchoolYearId(long schoolYearId)
        {
            try
            {
                var curriculumsByYear = await context.Set<SchoolYearCurriculum>()
                    .AsNoTracking()
                    .Where(syc => syc.SchoolYearId == schoolYearId)
                    .Include(syc => syc.Curriculum) 
                    .Select(syc => new SchoolYearCurriculumDTO
                    {
                        Id = syc.Id, 
                        AcademicYear = syc.SchoolYearNumber, 
                        Resolution = syc.Curriculum != null ? syc.Curriculum.Resolution : string.Empty
                    })
                    .ToListAsync();

                return new ResponseDTO<List<SchoolYearCurriculumDTO>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Años académicos del ciclo lectivo obtenidos exitosamente.",
                    Object = curriculumsByYear
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los años del ciclo lectivo: {ex.Message}");

                return new ResponseDTO<List<SchoolYearCurriculumDTO>>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al obtener los años académicos.",
                    Object = null
                };
            }
        }

        //REVISAR
        public async Task<ResponseDTO<SchoolYearByGradePostDTO>> PostByGrade(SchoolYearByGradePostDTO schoolYear)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                
                var schoolYearEntity = new SchoolYear
                {
                    Id = schoolYear.Id,
                    SchoolYearNumber = schoolYear.SchoolYearNumber,
                    CreatedBy = schoolYear.CreatedById ?? Guid.Empty,
                };

                await context.Set<SchoolYear>().AddAsync(schoolYearEntity);
                await context.SaveChangesAsync();

                
                var schoolYearPlans = new List<SchoolYearCurriculum>();

                if (schoolYear.CurriculumYear1 > 0)
                {
                    schoolYearPlans.Add(new SchoolYearCurriculum { SchoolYearId = schoolYearEntity.Id, CurriculumId = schoolYear.CurriculumYear1, SchoolYearNumber = 1, CreatedBy = schoolYear.CreatedById ?? Guid.Empty });
                }
                if (schoolYear.CurriculumYear2 > 0)
                {
                    schoolYearPlans.Add(new SchoolYearCurriculum { SchoolYearId = schoolYearEntity.Id, CurriculumId = schoolYear.CurriculumYear2, SchoolYearNumber = 2, CreatedBy = schoolYear.CreatedById ?? Guid.Empty });
                }
                if (schoolYear.CurriculumYear3 > 0)
                {
                    schoolYearPlans.Add(new SchoolYearCurriculum { SchoolYearId = schoolYearEntity.Id, CurriculumId = schoolYear.CurriculumYear3, SchoolYearNumber = 3, CreatedBy = schoolYear.CreatedById ?? Guid.Empty });
                }

                await context.Set<SchoolYearCurriculum>().AddRangeAsync(schoolYearPlans);
                await context.SaveChangesAsync();

               
                var subjects = new List<Subject>();

                if (schoolYear.CurriculumYear1 > 0)
                {
                    var subs1 = await context.Set<Subject>()
                        .Where(s => s.CurriculumId == schoolYear.CurriculumYear1 && s.Year == 1)
                        .ToListAsync();
                    subjects.AddRange(subs1);
                }

                if (schoolYear.CurriculumYear2 > 0)
                {
                    var subs2 = await context.Set<Subject>()
                        .Where(s => s.CurriculumId == schoolYear.CurriculumYear2 && s.Year == 2)
                        .ToListAsync();
                    subjects.AddRange(subs2);
                }

                if (schoolYear.CurriculumYear3 > 0)
                {
                    var subs3 = await context.Set<Subject>()
                        .Where(s => s.CurriculumId == schoolYear.CurriculumYear3 && s.Year == 3)
                        .ToListAsync();
                    subjects.AddRange(subs3);
                }

                var subjectIds = subjects.Select(s => s.Id).ToList();

               
                var divisionTemplates = await context.Set<DivisionTemplate>()
                    .Where(dt => subjectIds.Contains(dt.SubjectId))
                    .ToListAsync();

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
                    .Where(st => divisionTemplates.Select(dt => dt.Id).Contains(st.DivisionTemplateId))
                    .ToListAsync();

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

                
                await transaction.CommitAsync();

                return new ResponseDTO<SchoolYearByGradePostDTO>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "Ciclo lectivo creado exitosamente.",
                    Object = schoolYear
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al crear el ciclo lectivo: {ex.Message}");

                return new ResponseDTO<SchoolYearByGradePostDTO>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al crear el ciclo lectivo.",
                    Object = null
                };
            }
        }
    }
}
