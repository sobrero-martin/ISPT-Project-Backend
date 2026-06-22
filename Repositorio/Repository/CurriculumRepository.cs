using BD;
using BD.Entidades;
using DTO.DTOs;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<CurriculumDTO>> GetByCareer(long careerId)
        {
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
                return curriculumDTOs;
  
        }

        public async Task<CurriculumDTO> GetById (long id)
        {
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
            };
        }

        public async Task<CurriculumDTO> Post(Curriculum curriculum)
        {
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
            }
        }

        public async Task<bool> Put(long id, Curriculum curriculum)
        {
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
            return true;
        }
    }

}
