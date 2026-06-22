using BD;
using BD.Entidades;
using DTO.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Repository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext context;

        public SubjectRepository (AppDbContext context)
        {
            this.context = context;
        }

        /* DTO
        public required string Code { get; set; }

        public required string Name { get; set; }

        public int Year { get; set; }

        public required string Format { get; set; }
        */
        public async Task<List<SubjectDTO>> GetByCurriculum(long curriculumId)
        {
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
            return subjectDTOs;

        }

        public async Task<SubjectDTO> GetById (long id)
        {
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
            };
        }

        public async Task<SubjectDTO> Post(Subject subject)
        {
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
            }
        }

        public async Task<bool> Put(long id, Subject subject)
        {
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
            return true;
        }
    }

}

