using System.Net;
using BD;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.StudentsDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Repositorio.Repository;

public class StudentRepository : PersonRepository, IStudentRepository
{
    private readonly AppDbContext bbdd;
    private readonly UserManager<IdentityUser> userManager;
    
    public StudentRepository(AppDbContext bbdd, UserManager<IdentityUser> userManager) 
        : base(bbdd, userManager, "Estudiante", "estudiante", "estudiantes")
    {
        this.bbdd = bbdd;
        this.userManager = userManager;
    }
    
    public async Task<ResponseDTO<List<StudentDTO>>> GetAllStudents()
    {
         try
         {
             var peopleIdentity = await userManager.GetUsersInRoleAsync("Estudiante");
             var peopleId = peopleIdentity.Select(p => p.Id).ToList();

             var people = await bbdd.People
                 .AsNoTracking()
                 .Where(p => peopleId.Contains(p.UserId))
                 .Select(p => new StudentDTO()
                 {
                     Id = p.Id,
                     Firstname = p.Firstname,
                     Lastname = p.Lastname,
                     TypeDocument = p.TypeDocument,
                     DocumentNumber = p.DocumentNumber,
                 })
                 .ToListAsync();

             return new ResponseDTO<List<StudentDTO>>()
             {
                 StatusCode = HttpStatusCode.OK,
                 Message = "¡Listado de estudiantes obtenido con éxito!",
                 Object = people
             };
         }
         catch (Exception e)
         {
             Console.WriteLine($"Error al intentar obtener el listado de estudiantes: " + e.Message);
             return new ResponseDTO<List<StudentDTO>>()
             {
                 StatusCode = HttpStatusCode.InternalServerError,
                 Message = "¡Hubo un error al intentar obtener el listado de estudiantes!",
                 Object = null
             };
         }
    }
}