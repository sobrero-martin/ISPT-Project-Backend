using System.Net;
using BD;
using Microsoft.AspNetCore.Identity;


namespace Repositorio.Repository;

public class StudentRepository : PersonRepository, IStudentRepository
{
    public StudentRepository(AppDbContext bbdd, UserManager<IdentityUser> userManager) 
        : base(bbdd, userManager, "Estudiante", "estudiante", "estudiantes")
    {
    }
    
}