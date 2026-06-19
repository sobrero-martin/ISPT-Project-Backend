using BD;
using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
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

        public async Task<ResponseDTO<List<CareerDTO>>> GetFull()
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
            /*
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
            return careerDTOs;*/
        }

        public async Task<ResponseDTO<CareerDTO>> GetById(long id)
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

            /*
            var career = await context.Set<Career>().FirstOrDefaultAsync(x => x.Id == id);
            if (career == null) return null;
            return new CareerDTO
            {
                Id = career.Id,
                Name = career.Name,
                Title = career.Title
            };*/
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

            /*
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
            }*/
        }

        public async Task<ResponseDTO<string>> Put(long id, Career carrera)
        {
            try
            {
                if (id != carrera.Id)
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "El ID proporcionado no coincide con el ID de la carrera.",
                        Object = null
                    };
                }

                var existingCareer = await context.Set<Career>().FirstOrDefaultAsync(c => c.Id == id);

                if (existingCareer == null)
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "Carrera no encontrada.",
                        Object = null
                    };
                }

                existingCareer.Name = carrera.Name;
                existingCareer.Title = carrera.Title;

                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Carrera actualizada exitosamente.",
                    Object = null
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la carrera: {ex.Message}");

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al actualizar la carrera.",
                    Object = null
                };
            }

            
        }
    }
}
