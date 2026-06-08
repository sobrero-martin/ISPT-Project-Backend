using BD;
using BD.Entidades;
using DTO.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Repository
{
    public class CareerRepository : ICareerRepository
    {
        private readonly AppDbContext context;

        public CareerRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<CareerDTO>> GetFull()
        {
            var careers = await context.Set<Career>().ToListAsync();
            var careerDTOs = new List<CareerDTO>();
            foreach (var career in careers)
            {
                careerDTOs.Add(new CareerDTO
                {
                    Id = career.Id,
                    Name = career.Name,
                    Title = career.Title
                });
            }
            return careerDTOs;
        }

        public async Task<CareerDTO> GetById(long id)
        {
            var career = await context.Set<Career>().FirstOrDefaultAsync(x => x.Id == id);
            if (career == null) return null;
            return new CareerDTO
            {
                Id = career.Id,
                Name = career.Name,
                Title = career.Title
            };
        }

        public async Task<CareerDTO> Post(Career carrera)
        {
            try
            {
                await context.Set<Career>().AddAsync(carrera);
                await context.SaveChangesAsync();
                CareerDTO careerDTO = new CareerDTO
                {
                    Id = carrera.Id,
                    Name = carrera.Name,
                    Title = carrera.Title
                };
                return careerDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Put(long id, Career carrera)
        {
            if (id != carrera.Id) return false;

            bool existe = await context.Set<Career>().AnyAsync(x => x.Id == id);
            if (!existe) return false;

            try
            {
                context.Set<Career>().Update(carrera);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { throw; }
        }
    }
}
