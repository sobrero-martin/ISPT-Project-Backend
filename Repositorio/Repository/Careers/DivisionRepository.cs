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
    public class DivisionRepository //: IDivisionRepository
    {
        private readonly AppDbContext context;

        public DivisionRepository(AppDbContext context)
        {
            this.context = context;
        }

        /*DivisionDTO:
        public long Id { get; set; }
        public long SubjectId { get; set; }
        public long SchoolYearId { get; set; }
        public required string Name { get; set; }
        public required string DivisionState { get; set; }
*/
        /*
        public async Task<ResponseDTO<List<DivisionDTO>>> GetBySubject(long subjectId)
        {
            try
            {
                var subject = await context.Set<Subject>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == subjectId);

                if (subject == null)
                {
                    return new ResponseDTO<List<DivisionDTO>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Materia no encontrada."
                    };
                }

                var divisions = await context.Set<Division>()
                    .AsNoTracking()
                    .Where(s => s.SubjectId == subjectId)
                    .Select(s => new DivisionDTO
                    {
                        Id = s.Id,
                        SubjectId = s.SubjectId,
                        SchoolYearId = s.SchoolYearId,
                        Name = s.Name,
                        DivisionState = s.DivisionState
                    })
                    .ToListAsync();

                return new ResponseDTO<List<DivisionDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = divisions,
                    Message = "Divisiones obtenidas exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener divisiones por materia: {ex.Message}");

                return new ResponseDTO<List<DivisionDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener las divisiones."
                };
            }
        }*/
        /*
        public async Task<ResponseDTO<DivisionDTO>> Post(long subjectId)
        {
            try
            {
                var subject = await context.Set<Subject>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == subjectId);

                if (subject == null)
                {
                    return new ResponseDTO<DivisionDTO>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Materia no encontrada."
                    };
                }

                var divisions = await context.Set<Division>()
                    .AsNoTracking()
                    .Where(s => s.SubjectId == subjectId)
                    .ToListAsync();

                var existsA = false;
                var existsB = false;
                var existsC = false;
                var existsD = false;

                foreach (var division in divisions)
                {
                    if (division.Name == "A")
                    {
                        existsA = true; break;
                    }

                    if (division.Name == "B") {
                        existsB = true; break;
                    }

                    if (division.Name == "C") {
                        existsC = true; break;
                    }

                    if (division.Name == "D") {
                        existsD = true; break;
                    }
                }

                if (!existsA)
                {
                    var newDivision = new Division
                    {
                        SubjectId = subjectId,
                        SchoolYearId = 1,
                        Name = "A",
                        DivisionState = "Activo"
                    };
                    context.Set<Division>().Add(newDivision);
                    await context.SaveChangesAsync();
                    return new ResponseDTO<DivisionDTO>
                    {
                        StatusCode = System.Net.HttpStatusCode.Created,
                        Object = new DivisionDTO
                        {
                            Id = newDivision.Id,
                            SubjectId = newDivision.SubjectId,
                            SchoolYearId = newDivision.SchoolYearId,
                            Name = newDivision.Name,
                            DivisionState = newDivision.DivisionState
                        },
                        Message = "División A creada exitosamente."
                    };

                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }*/
    }
}
