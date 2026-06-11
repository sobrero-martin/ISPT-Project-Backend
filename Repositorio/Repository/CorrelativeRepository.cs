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
    public class CorrelativeRepository : ICorrelativeRepository
    {
        private readonly AppDbContext context;

        public CorrelativeRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<CorrelativeDTO> Post(Correlative correlative)
        {
            try
            {
                await context.Set<Correlative>().AddAsync(correlative);
                await context.SaveChangesAsync();
                CorrelativeDTO correlativeDTO = new CorrelativeDTO
                {
                    Id = correlative.Id,
                    SubjectId = correlative.SubjectId,
                };
                return correlativeDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Delete(long id)
        {
            return true;
        }

        public async Task<bool> Exists(long subjectId1, long subjectId2)
        {
            return true;
        }

    }
}
