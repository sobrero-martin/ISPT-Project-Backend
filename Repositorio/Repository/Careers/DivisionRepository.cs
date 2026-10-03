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
    public class DivisionRepository : IDivisionRepository
    {
        private readonly AppDbContext context;

        public DivisionRepository(AppDbContext context)
        {
            this.context = context;
        }
        public async Task<ResponseDTO<List<DivisionDTO>>> GetBySchoolYearSubject(long schoolYearId, long subjectId)
        {
            try
            {
                var baseSchoolYear = await context.Set<SchoolYear>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(sy => sy.Id == schoolYearId);

                if (baseSchoolYear == null)
                {
                    return new ResponseDTO<List<DivisionDTO>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Ciclo lectivo no encontrado."
                    };
                }

                var subject = await context.Set<Subject>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == subjectId);

                if (subject == null)
                {
                    return new ResponseDTO<List<DivisionDTO>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Espacio curricular no encontrado."
                    };
                }

                var targetSchoolYearId = await context.Set<SchoolYear>()
                    .Where(sy => sy.CurriculumId == baseSchoolYear.CurriculumId &&
                                 sy.SchoolYearNumber == baseSchoolYear.SchoolYearNumber &&
                                 sy.YearNumber == subject.Year) 
                    .Select(sy => sy.Id)
                    .FirstOrDefaultAsync();

                var divisionTemplates = await context.Set<DivisionTemplate>()
                    .Where(dt => dt.SubjectId == subjectId)
                    .ToListAsync();

                var divisions = await context.Set<Division>()
                    .Where(d => d.SchoolYearId == targetSchoolYearId && divisionTemplates.Select(dt => dt.Id).Contains(d.DivisionTemplateId))
                    .Select(d => new DivisionDTO
                    {
                        Id = d.Id,
                        Name = d.DivisionTemplate!.Name,
                    })
                    .ToListAsync();

                return new ResponseDTO<List<DivisionDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = divisions,
                    Message = "Divisiones obtenidas correctamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las divisiones: {ex.Message}");
                return new ResponseDTO<List<DivisionDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener las divisiones."
                };
            }
        }

    }
    }
