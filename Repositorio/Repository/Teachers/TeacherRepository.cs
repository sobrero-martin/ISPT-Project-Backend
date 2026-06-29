using System.Net;
using BD;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.TeachersDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repositorio.Repository;

public class TeacherRepository : PersonRepository, ITeacherRepository
{
    private readonly AppDbContext bbdd;
    private readonly UserManager<IdentityUser> userManager;
    
    public TeacherRepository(AppDbContext bbdd, UserManager<IdentityUser> userManager) : 
        base(bbdd, userManager, "Docente", "docente", "docentes")
    {
        this.bbdd = bbdd;
        this.userManager = userManager;
    }
    
    public async Task<ResponseDTO<List<TeacherDTO>>> GetAllTeachers()
    {
        try
        {
            var peopleIdentity = await userManager.GetUsersInRoleAsync("Docente");
            var peopleId = peopleIdentity.Select(p => p.Id).ToList();

            var people = await bbdd.People
                .AsNoTracking()
                .Where(p => peopleId.Contains(p.UserId))
                .Select(p => new TeacherDTO()
                {
                    Id = p.Id,
                    Firstname = p.Firstname,
                    Lastname = p.Lastname,
                    DocumentNumber = p.DocumentNumber,
                })
                .ToListAsync();

            return new ResponseDTO<List<TeacherDTO>>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Listado de docentes obtenido con éxito!",
                Object = people
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al intentar obtener el listado de docentes: " + e.Message);
            return new ResponseDTO<List<TeacherDTO>>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar obtener el listado de docentes!",
                Object = null
            };
        }
    }
}