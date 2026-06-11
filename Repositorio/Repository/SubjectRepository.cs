using BD;
using BD.Entidades;
using DTO.DTOs.CareerDTO;
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

        public async Task<SubjectDTO> GetById(long id)
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

        public async Task<List<SubjectDTO>> GetPossibleCorrelatives(long curriculumId, long subjectId)
        {
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
            return subjectDTOs;
        }
    }
}



