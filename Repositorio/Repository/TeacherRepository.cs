using BD;
using Microsoft.AspNetCore.Identity;

namespace Repositorio.Repository;

public class TeacherRepository : PersonRepository, ITeacherRepository
{
    public TeacherRepository(AppDbContext bbdd, UserManager<IdentityUser> userManager) 
        : base(bbdd, userManager, "Docente", "docente", "docentes")
    {
    }
}