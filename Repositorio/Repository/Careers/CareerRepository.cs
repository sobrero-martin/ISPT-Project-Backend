using BD;
using BD.Entidades;
using DTO.DTOs.CareerDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Careers;
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

        }

        public async Task<ResponseDTO<CareerDTO>> GetById(long id)
        {
            try
            {
                var career = await context.Set<Career>()
                    .AsNoTracking()
                    .Where(c => c.Id == id)
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

        public async Task<ResponseDTO<string>> Post(CareerPostDTO careerPostDTO)
        {
            try
            {
                var careerEntity = new Career
                {
                    Id = careerPostDTO.Id,
                    Name = careerPostDTO.Name,
                    Title = careerPostDTO.Title,
                    CreatedBy = careerPostDTO.CreatedById ?? Guid.Empty
                };

                await context.Set<Career>().AddAsync(careerEntity);
                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.Created,
                    Message = "Operación exitosa.",
                    Object = $"¡Carrera:{careerPostDTO.Name} creada con éxito!"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la carrera: {ex.Message}");

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = "Ocurrió un error al crear la carrera.",
                    Object = null
                };
            }
        }

        public async Task<ResponseDTO<string>> Put(long id, CareerPostDTO careerPostDTO)
        {
            try
            {
                if (id != careerPostDTO.Id)
                {
                    throw new Exception("IdMismatch");         
                }

                var existingCareer = await context.Set<Career>().FirstOrDefaultAsync(c => c.Id == id);

                if (existingCareer == null)
                {
                    throw new Exception("careerNotFound");
                }

                existingCareer.Name = careerPostDTO.Name;
                existingCareer.Title = careerPostDTO.Title;
                existingCareer.UpdatedBy = careerPostDTO.UpdatedById ?? Guid.Empty;

                await context.SaveChangesAsync();

                return new ResponseDTO<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Operación exitosa.",
                    Object = $"Carrera:{careerPostDTO.Name} actualizada con éxito"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la carrera: {ex.Message}");

                if (ex.Message == "IdMismatch")
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "El ID proporcionado no coincide con el ID de la carrera",
                        Object = $"ID proporcionado: {careerPostDTO.Id}, ID de la carrera: {id}."
                    };
                }

                if(ex.Message == "careerNotFound")
                {
                    return new ResponseDTO<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "Carrera no encontrada.",
                        Object = null
                    };
                }


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