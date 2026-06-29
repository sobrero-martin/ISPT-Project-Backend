using System.Net;
using BD;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.PersonDTO;
using DTO.DTOs.PositionsDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositorio.Repository.Positions;

namespace Repositorio.Repository;

public class PositionRepository : PersonRepository, IPositionRepository
{
    private readonly AppDbContext bbdd;
    private readonly UserManager<IdentityUser> userManager;
    private static List<string> roles = ["Directivo", "Preceptor", "Preceptor_Auxiliar"];

    private Dictionary<string, string> roleFriendlyNames = new()
    {
        { "Directivo", "Directivo" },
        { "Preceptor", "Preceptor" },
        { "Preceptor_Auxiliar", "Preceptor Auxiliar" }
    };

    public PositionRepository(AppDbContext bbdd, UserManager<IdentityUser> userManager)
        : base(bbdd, userManager, roles, "persona con cargo", "cargos")
    {
        this.bbdd = bbdd;
        this.userManager = userManager;
    }

    public async Task<ResponseDTO<List<PositionDTO>>> GetAllPersonal()
    {
        try
        {
            var userRoleMapping = new Dictionary<string, string>();

            foreach (var rol in roles)
            {
                var users = await userManager.GetUsersInRoleAsync(rol);
                var friendlyRolName = roleFriendlyNames.GetValueOrDefault(rol, rol);

                foreach (var user in users)
                {
                    if (!userRoleMapping.ContainsKey(user.Id)) userRoleMapping.Add(user.Id, friendlyRolName);
                    else userRoleMapping[user.Id] += $", {friendlyRolName}";
                }
            }

            var peopleId = userRoleMapping.Keys.ToList();

            var people = await bbdd.People
                .AsNoTracking()
                .Where(p => peopleId.Contains(p.UserId))
                .ToListAsync();

            var resultList = people.Select(p => new PositionDTO()
            {
                Id = p.Id,
                DocumentNumber = p.DocumentNumber,
                Firstname = p.Firstname,
                Lastname = p.Lastname,
                RoleName = userRoleMapping!.GetValueOrDefault(p.UserId, "")
            }).ToList();

            return new ResponseDTO<List<PositionDTO>>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Listado de personas con cargo obtenido con éxito!",
                Object = resultList
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al intentar obtener el listado de personas con cargo: " + e.Message);
            return new ResponseDTO<List<PositionDTO>>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar obtener el listado de personas con cargo!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<PersonDTO>> GetPositionById(long id)
    {
        try
        {
            var p = await bbdd.People.AsNoTracking()
                .Include(p => p.Location)
                .Include(p => p.Contact)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) throw new Exception("No hay una persona con cargo con ese ID.");

            var user = await userManager.FindByIdAsync(p.UserId);
            string roleName = "";
            if (user != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var matchedRoles = userRoles
                    .Where(r => roles.Contains(r))
                    .Select(r => roleFriendlyNames.ContainsKey(r) ? roleFriendlyNames[r] : r);
                roleName = string.Join(", ", matchedRoles);
            }

            return new ResponseDTO<PersonDTO>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = new PersonDTO()
                {
                    Firstname = p.Firstname,
                    Lastname = p.Lastname,
                    CUIL = p.CUIL,
                    TypeDocument = p.TypeDocument,
                    DocumentNumber = p.DocumentNumber,
                    Birthdate = p.Birthdate,
                    Gender = p.Gender,
                    LocationDTO = new LocationDTO()
                    {
                        Address = p.Location.Address,
                        Country = p.Location.Country,
                        Department = p.Location.Department,
                        Province = p.Location.Province,
                    },
                    ContactDTO = new ContactDTO()
                    {
                        ContactNameEmergency = p.Contact.ContactNameEmergency,
                        EmergencyNumber = p.Contact.EmergencyNumber,
                        PhoneNumber = p.Contact.PhoneNumber,
                        Email = p.Contact.Email,
                    },
                    Observations = p.Observations,
                    PracticePlace = p.PracticePlace,
                    CreatedById = Guid.Empty,
                    RoleName = roleName
                }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al intentar obtener una persona con cargo: " + e.Message);
            return new ResponseDTO<PersonDTO>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar obtener a la persona con cargo!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> RemovePersonWithPosition(long id)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var p = await bbdd.People.AsNoTracking() .FirstOrDefaultAsync(p => p.Id == id);
            if (p == null) throw new Exception("No hay una persona con cargo con ese ID.");
            
            var user = await userManager.FindByIdAsync(p.UserId);
            if (user == null) throw new Exception("El usuario de Identity asociado no existe.");
            
            var userRoles = await userManager.GetRolesAsync(user);
            var rolesToRemove = userRoles.Where(r => roles.Contains(r)).ToList();
            
            if (rolesToRemove.Any())
            {
                var result = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"No se pudieron quitar los roles. Errores: {errors}");
                }
            }
            
            await transaction.CommitAsync();
            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Persona con cargo removida con éxito!",
                Object = "Éxito"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error al intentar remover a la persona con cargo: " + e.Message);

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar remover a la persona con cargo!",
                Object = null
            };
        }
    }
}