using BD;
using Microsoft.AspNetCore.Identity;

namespace Repositorio.Repository;

public class PositionRepository : PersonRepository
{
    public PositionRepository(AppDbContext bbdd, UserManager<IdentityUser> userManager) 
        : base(bbdd, userManager, new List<string> { "Directivo", "Preceptor", "Preceptor_Auxiliar" }, "", "cargos")
    {
    }
}