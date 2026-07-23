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
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext context;

        public SubjectRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<List<SubjectTableDTO>>> GetByCurriculum(long curriculumId)
        {
            try
            {
                var curriculum = await context.Set<Curriculum>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == curriculumId);

                if (curriculum == null)
                {
                    return new ResponseDTO<List<SubjectTableDTO>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Plan de estudio no encontrado."
                    };
                }

                var subjects = await context.Set<Subject>()
                    .AsNoTracking()
                    .Where(s => s.CurriculumId == curriculumId)
                    .Select(s => new SubjectTableDTO()
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Name = s.Name,
                        Year = s.Year,
                        Format = s.Format,
                    })
                    .ToListAsync();

                return new ResponseDTO<List<SubjectTableDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = subjects,
                    Message = "Materias obtenidas exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener materias por plan de estudio: {ex.Message}");

                return new ResponseDTO<List<SubjectTableDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener las materias."
                };
            }

        }

        public async Task<ResponseDTO<SubjectDTO>> GetById(long id)
        {
            try
            {
                var subject = await context.Set<Subject>()
                    .AsNoTracking()
                    .Where(s => s.Id == id)
                    .Select(s => new SubjectDTO
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Name = s.Name,
                        Year = s.Year,
                        Format = s.Format,
                        Type = s.Type,
                        ContactHour = s.ContactHour
                    })
                    .FirstOrDefaultAsync(x => x.Id == id);

                if(subject == null)
                {
                    return new ResponseDTO<SubjectDTO>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Materia no encontrada."
                    };
                }

                return new ResponseDTO<SubjectDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = subject,
                    Message = "Materia obtenida exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener materia por ID: {ex.Message}");

                return new ResponseDTO<SubjectDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener la materia."
                };
            }

        }

        public async Task<ResponseDTO<SubjectDTO>> Post(SubjectPostDTO subject)
        {
            try
            {
                var newSubject = new Subject
                {
                    CurriculumId = subject.CurriculumId,
                    Code = subject.Code,
                    Name = subject.Name,
                    Year = subject.Year,
                    Format = subject.Format,
                    Type = subject.Type,
                    ContactHour = subject.ContactHour,
                    CreatedBy = subject.CreatedById ?? Guid.Empty
                };

                await context.Set<Subject>().AddAsync(newSubject);
                await context.SaveChangesAsync();

                return new ResponseDTO<SubjectDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.Created,
                    Object = new SubjectDTO
                    {
                        Code = newSubject.Code,
                        Name = newSubject.Name,
                        Year = newSubject.Year,
                        Format = newSubject.Format,
                        Type = newSubject.Type,
                        ContactHour = newSubject.ContactHour
                    },
                    Message = "Materia creada exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine($"Error al crear materia: {ex.Message}");

                if (ex.Message.Contains("Duplicate entry"))
                {
                    return new ResponseDTO<SubjectDTO>
                    {
                        StatusCode = System.Net.HttpStatusCode.Conflict,
                        Object = null,
                        Message = "¡El código de la materia  ya existe en el sistema, no puede haber duplicados!"
                    };
                }
                
                return new ResponseDTO<SubjectDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al crear la materia."
                };
            }

        }

        public async Task<ResponseDTO<string>> Put(long id, SubjectPostDTO subject)
        {
            try
            {
                if(id != subject.Id)
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Object = null,
                        Message = "El ID de la materia no coincide con el ID proporcionado."
                    };
                }

                var existingSubject = await context.Set<Subject>().FirstOrDefaultAsync(x => x.Id == id);

                if(existingSubject == null)
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Materia no encontrada."
                    };
                }

                if (context.Subjects.Any(x => x.Code == subject.Code)) throw new Exception("Duplicate entry");

                existingSubject.Code = subject.Code;
                existingSubject.Name = subject.Name;
                existingSubject.Year = subject.Year;
                existingSubject.Format = subject.Format;
                existingSubject.Type = subject.Type;
                existingSubject.ContactHour = subject.ContactHour;
                existingSubject.UpdatedBy = subject.UpdatedById ?? Guid.Empty;

                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = null,
                    Message = "Materia actualizada exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar materia: {ex.Message}");

                if (ex.Message.Contains("Duplicate entry"))
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = System.Net.HttpStatusCode.Conflict,
                        Object = null,
                        Message = "¡El código de la materia  ya existe en el sistema, no puede haber duplicados!"
                    };
                }
                
                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al actualizar la materia."
                };
            }
        }

        public async Task<ResponseDTO<List<SubjectCorrelativesDTO>>> GetPossibleCorrelatives(long curriculumId, long subjectId)
        {
            try
            {
                var subjectYear = await context.Set<Subject>()
                    .AsNoTracking()
                    .Where(s => s.Id == subjectId)
                    .Select(s => s.Year)
                    .FirstOrDefaultAsync();

                var correlatives = await context.Set<Correlative>()
                    .AsNoTracking()
                    .Where(c => c.SubjectId == subjectId)
                    .Select(c => c.SubjectCorrelativeId)
                    .ToListAsync();

                var curriculum = await context.Set<Curriculum>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == curriculumId);

                if (curriculum == null)
                {
                    return new ResponseDTO<List<SubjectCorrelativesDTO>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Plan de estudio no encontrado."
                    };
                }

                    var subjects = await context.Set<Subject>()
                    .AsNoTracking()
                    .Where(s => s.CurriculumId == curriculumId && s.Year < subjectYear)
                    .Select(s => new SubjectCorrelativesDTO
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Name = s.Name,
                        Format = s.Format,
                        IsCorrelative = correlatives.Contains(s.Id)
                    })
                    .ToListAsync();

                return new ResponseDTO<List<SubjectCorrelativesDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = subjects,
                    Message = "Materias obtenidas exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener posibles correlativas: {ex.Message}");

                return new ResponseDTO<List<SubjectCorrelativesDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener las posibles correlativas."
                };
            }

        }

        public async Task<ResponseDTO<List<SubjectTableDTO>>> GetBySchoolYear(long schoolYearId)
        {
            try
            {
                var schoolYear = await context.Set<SchoolYear>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == schoolYearId);

                if (schoolYear == null)
                {
                    return new ResponseDTO<List<SubjectTableDTO>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Ciclo lectivo no encontrado."
                    };
                }

                var subjects = await context.Set<Subject>()
                    .AsNoTracking()
                    .Where(s => s.CurriculumId == schoolYear.CurriculumId)
                    .Select(s => new SubjectTableDTO
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Name = s.Name,
                        Year = s.Year,
                        Format = s.Format
                    })
                    .ToListAsync();

                return new ResponseDTO<List<SubjectTableDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = subjects,
                    Message = "Materias obtenidas exitosamente."
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener materias por año escolar: {ex.Message}");

                return new ResponseDTO<List<SubjectTableDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener las materias."
                };
            }
        }
    }
}



