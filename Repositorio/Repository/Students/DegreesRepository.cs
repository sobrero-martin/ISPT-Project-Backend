using System.Net;
using BD;
using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.StudentsDTO;
using Microsoft.EntityFrameworkCore;

namespace Repositorio.Repository;

public class DegreesRepository : IDegreesRepository
{
    private readonly AppDbContext bbdd;
    
    public DegreesRepository(AppDbContext bbdd) 
    {
        this.bbdd = bbdd;
    }

    public async Task<ResponseDTO<List<DegreeDTO>>> GetDegreesByPersonId(long id)
    {
        try
        {
            var degreeList = await bbdd.Degrees.Where(d => d.PersonId == id).Select(d => new DegreeDTO()
            {
                Id = d.Id,
                personId =  d.PersonId,
                Name =  d.TitleName
            }).ToListAsync();

            return new ResponseDTO<List<DegreeDTO>>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = degreeList
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al intentar obtener el listado de títulos/certificados: " + e.Message);
            return new ResponseDTO<List<DegreeDTO>>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"¡Hubo un error al intentar obtener el listado de títulos/certificados!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> createDegree(DegreeDTO degreeDTO)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            bbdd.Degrees.Add(new Degree()
            {
                PersonId = degreeDTO.personId,
                TitleName = degreeDTO.Name
            });
            
            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Título/Certificado añadido con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error al intentar crear un título: " + e.Message);
            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar crear el título!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> deleteDegree(long id)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var degree = await bbdd.Degrees.FindAsync(id);
            if (degree == null) throw new Exception("El título/certificado que desea borrar no existe.");
            
            bbdd.Degrees.Remove(degree);
            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Título/Certificado eliminado con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error al intentar eliminar un título: " + e.Message);
            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"¡Hubo un error al intentar eliminar un título!",
                Object = null
            };
        }
    }
}