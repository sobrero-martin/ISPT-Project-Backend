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
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext context;

        public SubjectRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ResponseDTO<List<SubjectDTO>>> GetByCurriculum(long curriculumId)
        {
            try
            {
                var curriculum = await context.Set<Curriculum>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == curriculumId);

                if (curriculum == null)
                {
                    return new ResponseDTO<List<SubjectDTO>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Plan de estudio no encontrado."
                    };
                }

                var subjects = await context.Set<Subject>()
                    .AsNoTracking()
                    .Where(s => s.CurriculumId == curriculumId)
                    .Select(s => new SubjectDTO
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Name = s.Name,
                        Year = s.Year,
                        Format = s.Format,
                        Type = s.Type,
                        Duration = s.Duration
                    })
                    .ToListAsync();

                return new ResponseDTO<List<SubjectDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = subjects,
                    Message = "Materias obtenidas exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener materias por plan de estudio: {ex.Message}");

                return new ResponseDTO<List<SubjectDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener las materias."
                };
            }

            /*
            var subjects = await context.Set<Subject>().Where(c => c.CurriculumId == curriculumId).ToListAsync();
            var subjectDTOs = new List<SubjectDTO>();
            foreach (var subject in subjects)
            {
                subjectDTOs.Add(new SubjectDTO
                {
                    Id = subject.Id,
                    Code = subject.Code,
                    Name = subject.Name,
                    Year = subject.Year,
                    Format = subject.Format,
                    Type = subject.Type,
                    Duration = subject.Duration
                });
            }
            return subjectDTOs;*/

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
                        Duration = s.Duration
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
            /*
            var subject = await context.Set<Subject>().FirstOrDefaultAsync(x => x.Id == id);

            if (subject == null) return null;

            return new SubjectDTO
            {
                Id = subject.Id,
                Code = subject.Code,
                Name = subject.Name,
                Year = subject.Year,
                Format = subject.Format,
                Type = subject.Type,
                Duration = subject.Duration
            };*/
        }

        public async Task<ResponseDTO<SubjectDTO>> Post(Subject subject)
        {
            try
            {
                await context.Set<Subject>().AddAsync(subject);
                await context.SaveChangesAsync();

                return new ResponseDTO<SubjectDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.Created,
                    Object = new SubjectDTO
                    {
                        Id = subject.Id,
                        Code = subject.Code,
                        Name = subject.Name,
                        Year = subject.Year,
                        Format = subject.Format,
                        Type = subject.Type,
                        Duration = subject.Duration
                    },
                    Message = "Materia creada exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear materia: {ex.Message}");

                return new ResponseDTO<SubjectDTO>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al crear la materia."
                };
            }
            /*
            try
            {
                await context.Set<Subject>().AddAsync(subject);
                await context.SaveChangesAsync();
                SubjectDTO subjectDTO = new SubjectDTO
                {
                    Id = subject.Id,
                    Code = subject.Code,
                    Name = subject.Name,
                    Year = subject.Year,
                    Format = subject.Format,
                    Type = subject.Type,
                    Duration = subject.Duration
                };
                return subjectDTO;
            }
            catch (Exception)
            {
                throw;
            }*/
        }

        public async Task<ResponseDTO<string>> Put(long id, Subject subject)
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

                existingSubject.Code = subject.Code;
                existingSubject.Name = subject.Name;
                existingSubject.Year = subject.Year;
                existingSubject.Format = subject.Format;
                existingSubject.Type = subject.Type;
                existingSubject.Duration = subject.Duration;

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

                return new ResponseDTO<string>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al actualizar la materia."
                };
            }
            /*
            if (id != subject.Id) return false;

            var existing = await context.Set<Subject>()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null) return false;

            existing.Code = subject.Code;
            existing.Name = subject.Name;
            existing.Year = subject.Year;
            existing.Format = subject.Format;
            existing.Type = subject.Type;
            existing.Duration = subject.Duration;

            await context.SaveChangesAsync();
            return true;*/
        }

        public async Task<ResponseDTO<List<SubjectDTO>>> GetPossibleCorrelatives(long curriculumId, long subjectId)
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
                    return new ResponseDTO<List<SubjectDTO>>
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        Object = null,
                        Message = "Plan de estudio no encontrado."
                    };
                }

                    var subjects = await context.Set<Subject>()
                    .AsNoTracking()
                    .Where(s => s.CurriculumId == curriculumId && s.Year < subjectYear)
                    .Select(s => new SubjectDTO
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Name = s.Name,
                        Year = s.Year,
                        Format = s.Format,
                        Type = s.Type,
                        Duration = s.Duration,
                        IsCorrelative = correlatives.Contains(s.Id)
                    })
                    .ToListAsync();

                return new ResponseDTO<List<SubjectDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Object = subjects,
                    Message = "Materias obtenidas exitosamente."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener posibles correlativas: {ex.Message}");

                return new ResponseDTO<List<SubjectDTO>>
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Object = null,
                    Message = "Ocurrió un error al obtener las posibles correlativas."
                };
            }
            /*
            var subjectYear = await context.Set<Subject>().Where(s => s.Id == subjectId).Select(s => s.Year).FirstOrDefaultAsync();

            var subjects = await context.Set<Subject>().Where(s => s.CurriculumId == curriculumId && s.Year < subjectYear).ToListAsync();

            var correlatives = await context.Set<Correlative>().Where(c => c.SubjectId == subjectId).Select(c => c.SubjectCorrelativeId).ToListAsync();

            var subjectDTOs = new List<SubjectDTO>();
            foreach (var subject in subjects)
            {
                subjectDTOs.Add(new SubjectDTO
                {
                    Id = subject.Id,
                    Code = subject.Code,
                    Name = subject.Name,
                    Year = subject.Year,
                    Format = subject.Format,
                    Type = subject.Type,
                    Duration = subject.Duration,
                    IsCorrelative = correlatives.Contains(subject.Id)
                });
            }
            return subjectDTOs;*/
        }
    }
}



