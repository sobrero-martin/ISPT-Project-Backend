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
            try 
            { 
                var careers = await context.Set<Career>()
                    .AsNoTracking()
                    .Select(c => new CareerDTO 
                    { 
                        Id = c.Id,
                        Name = c.Name,
                        Title = c.Title
                    })
                    .ToListAsync();

                return new ResponseDTO<List<CareerDTO>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Listado de carreras obtenido exitosamente.",
                    Object = careers
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el listado de carreras: {ex.Message}");

                return new ResponseDTO<List<CareerDTO>>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al obtener el listado de carreras.",
                    Object = null
                };
            }

        }

        public async Task<CareerDTO> GetById(long id)
        {
            try
            {
                var career = await context.Set<Career>()
                    .AsNoTracking()
                    .Where(c  => c.Id == id)
                    .Select(c => new CareerDTO
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Title = c.Title
                    })
                    .FirstOrDefaultAsync();

                if (career == null)
                {
                    return new ResponseDTO<CareerDTO>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "Carrera no encontrada.",
                        Object = null
                    };
                }

                return new ResponseDTO<CareerDTO>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Carrera obtenida exitosamente.",
                    Object = career
                };
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error al obtener la carrera: {ex.Message}");

                return new ResponseDTO<CareerDTO>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al obtener la carrera.",
                    Object = null
                };
            }

        }

        public async Task<ResponseDTO<CareerDTO>> Post(Career carrera)
        {
            try 
            {
                await context.Set<Career>().AddAsync(carrera);
                await context.SaveChangesAsync();

                return new ResponseDTO<CareerDTO>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "Carrera creada exitosamente.",
                    Object = new CareerDTO
                    {
                        Id = carrera.Id,
                        Name = carrera.Name,
                        Title = carrera.Title
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la carrera: {ex.Message}");

                return new ResponseDTO<CareerDTO>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al crear la carrera.",
                    Object = null
                };
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
