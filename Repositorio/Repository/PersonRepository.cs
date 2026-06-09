using System.Net;
using BD;
using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.PersonDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repositorio.Repository;

public class PersonRepository : IPersonRepository
{
    private readonly AppDbContext bbdd;
    private readonly UserManager<IdentityUser> userManager;
    private readonly List<string> roleNames;
    private readonly string singularTypeName;
    private readonly string pluralTypeName;

    // ONLY ONE ROL
    public PersonRepository(AppDbContext bbdd, UserManager<IdentityUser> userManager,
        string roleName, string singularTypeName, string pluralTypeName)
    {
        this.bbdd = bbdd;
        this.userManager = userManager;
        roleNames = new List<string>() { roleName };
        this.singularTypeName = singularTypeName;
        this.pluralTypeName = pluralTypeName;
    }

    // MULTIPLE ROLES LIKE POSITION
    public PersonRepository(AppDbContext bbdd, UserManager<IdentityUser> userManager,
        List<string> roleNames, string singularTypeName, string pluralTypeName)
    {
        this.bbdd = bbdd;
        this.userManager = userManager;
        this.roleNames = roleNames;
        this.singularTypeName = singularTypeName;
        this.pluralTypeName = pluralTypeName;
    }

    public async Task<ResponseDTO<List<PersonDTO>>> GetAllPeopleByRol()
    {
        try
        {
            var peopleId = new List<string>();
            foreach (var role in roleNames)
            {
                var peopleIdentity = await userManager.GetUsersInRoleAsync(role);
                peopleId.AddRange(peopleIdentity.Select(p => p.Id));
            }

            peopleId = peopleId.Distinct().ToList();

            var people = await bbdd.Persons
                .AsNoTracking()
                .Include(p => p.Location)
                .Include(p => p.Contact)
                .Where(p => peopleId.Contains(p.UserId))
                .Select(p => new PersonDTO
                {
                    Id = p.Id,
                    CUIL = p.CUIL,
                    Firstname = p.Firstname,
                    Lastname = p.Lastname,
                    Birthdate = p.Birthdate,
                    TypeDocument = p.TypeDocument,
                    DocumentNumber = p.DocumentNumber,
                    Gender = p.Gender,
                    Observations = p.Observations,
                    PracticePlace = p.PracticePlace,
                    CreatedById = p.CreatedBy,
                    LocationDTO = new LocationDTO()
                    {
                        Address = p.Location.Address,
                        Province = p.Location.Province,
                        Department = p.Location.Department,
                        Country = p.Location.Country
                    },
                    ContactDTO = new ContactDTO
                    {
                        Email = p.Contact.Email,
                        PhoneNumber = p.Contact.PhoneNumber,
                        ContactNameEmergency = p.Contact.ContactNameEmergency,
                        EmergencyNumber = p.Contact.EmergencyNumber,
                    }
                })
                .ToListAsync();

            return new ResponseDTO<List<PersonDTO>>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = roleNames.Count > 1
                    ? $"¡Listado de personas con {pluralTypeName} obtenido con éxito!"
                    : $"¡Listado de {pluralTypeName} obtenido con éxito!",
                Object = people
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al intentar obtener el listado de {pluralTypeName}: " + e.Message);
            return new ResponseDTO<List<PersonDTO>>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = roleNames.Count > 1
                    ? $"¡Hubo un error al intentar obtener el listado de personas con {pluralTypeName}!"
                    : $"¡Hubo un error al intentar obtener el listado de {pluralTypeName}!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<PersonDTO>> GetPerson(long id)
    {
        try
        {
            var p = await bbdd.Persons.AsNoTracking()
                .Include(p => p.Location)
                .Include(p => p.Contact)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) throw new Exception($"No hay un {singularTypeName} con ese ID.");

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
                    CreatedById = Guid.Empty
                }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al intentar obtener un {singularTypeName}: " + e.Message);
            return new ResponseDTO<PersonDTO>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"¡Hubo un error al intentar obtener al {singularTypeName}!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> AddPerson(PersonDTO personDTO)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();

        try
        {
            var newUser = new IdentityUser()
            {
                UserName = personDTO.CUIL,
                NormalizedUserName = personDTO.CUIL,
                EmailConfirmed = true
            };

            var resultUsuario = await userManager.CreateAsync(newUser, personDTO.CUIL);
            if (!resultUsuario.Succeeded) throw new Exception("Error al crear el usuario");

            string roleToAsign = !string.IsNullOrEmpty(personDTO.RoleName)
                ? personDTO.RoleName
                : roleNames.First();

            var resultRol = await userManager.AddToRoleAsync(newUser, roleToAsign);
            if (!resultRol.Succeeded) throw new Exception("Error al asignar el rol");

            Person p = new()
            {
                CreatedBy = personDTO.CreatedById ?? Guid.Empty,
                UserId = newUser.Id,
                CUIL = personDTO.CUIL,
                Birthdate = personDTO.Birthdate,
                TypeDocument = personDTO.TypeDocument,
                DocumentNumber = personDTO.DocumentNumber,
                Firstname = personDTO.Firstname,
                Lastname = personDTO.Lastname,
                Gender = personDTO.Gender,
                Observations = personDTO.Observations,
                PracticePlace = personDTO.PracticePlace,
            };
            bbdd.Persons.Add(p);
            await bbdd.SaveChangesAsync();

            Location l = new()
            {
                Address = personDTO.LocationDTO.Address,
                Country = personDTO.LocationDTO.Country,
                Department = personDTO.LocationDTO.Department,
                Province = personDTO.LocationDTO.Province,
                PersonId = p.Id,
                CreatedBy = personDTO.CreatedById ?? Guid.Empty,
            };
            bbdd.Locations.Add(l);

            Contact c = new()
            {
                Email = personDTO.ContactDTO?.Email,
                PhoneNumber = personDTO.ContactDTO?.PhoneNumber,
                ContactNameEmergency = personDTO.ContactDTO?.ContactNameEmergency,
                EmergencyNumber = personDTO.ContactDTO?.EmergencyNumber,
                PersonId = p.Id,
                CreatedBy = personDTO.CreatedById ?? Guid.Empty,
            };
            bbdd.Contacts.Add(c);

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = $"¡{singularTypeName} creado con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error al intentar crear al {singularTypeName}: " + e.Message);
            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"¡Hubo un error al intentar crear al {singularTypeName}!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> EditPerson(PersonDTO personDTO)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var person = await bbdd.Persons.FindAsync(personDTO.Id);
            if (person == null) throw new Exception("La persona que se desea actualizar no existe.");

            var user = await userManager.FindByIdAsync(person.UserId);
            if (user == null) throw new Exception("La persona no cuenta con un usuario.");

            if (person.CUIL != personDTO.CUIL)
            {
                user.UserName = personDTO.CUIL;
                user.NormalizedUserName = personDTO.CUIL;

                var resultUsuario = await userManager.UpdateAsync(user);
                if (!resultUsuario.Succeeded) throw new Exception("Error al actualizar el usuario");
            }

            if (!string.IsNullOrEmpty(personDTO.RoleName))
            {
                var currentRoles = await userManager.GetRolesAsync(user);
                var rolesToRemove = currentRoles.Intersect(roleNames).ToList();

                if (rolesToRemove.Any())
                {
                    var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded) throw new Exception("Error al borrar los roles del usuario.");
                }

                var addResult = await userManager.AddToRoleAsync(user, personDTO.RoleName);
                if (!addResult.Succeeded) throw new Exception($"Error al asignar el nuevo rol: {personDTO.RoleName}");
            }

            person.CUIL = personDTO.CUIL;
            person.Birthdate = personDTO.Birthdate;
            person.TypeDocument = personDTO.TypeDocument;
            person.DocumentNumber = personDTO.DocumentNumber;
            person.Firstname = personDTO.Firstname;
            person.Lastname = personDTO.Lastname;
            person.Gender = personDTO.Gender;
            person.Observations = personDTO.Observations;
            person.PracticePlace = personDTO.PracticePlace;
            person.UpdatedBy = personDTO.UpdatedById ?? Guid.Empty;

            var location = await bbdd.Locations.FirstAsync(l => l.PersonId == person.Id);
            location.Address = personDTO.LocationDTO.Address;
            location.Country = personDTO.LocationDTO.Country;
            location.Department = personDTO.LocationDTO.Department;
            location.Province = personDTO.LocationDTO.Province;
            location.UpdatedBy = personDTO.UpdatedById ?? Guid.Empty;

            var contact = await bbdd.Contacts.FirstAsync(c => c.PersonId == person.Id);
            contact.Email = personDTO.ContactDTO?.Email;
            contact.PhoneNumber = personDTO.ContactDTO?.PhoneNumber;
            contact.ContactNameEmergency = personDTO.ContactDTO?.ContactNameEmergency;
            contact.EmergencyNumber = personDTO.ContactDTO?.EmergencyNumber;
            contact.UpdatedBy = personDTO.UpdatedById ?? Guid.Empty;

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = string.IsNullOrEmpty(personDTO.RoleName) 
                    ? $"¡{singularTypeName} actualizado con éxito!"
                    : "¡Persona con cargo actualizada con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error al intentar actualizar al {singularTypeName}: " + e.Message);
            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = string.IsNullOrEmpty(personDTO.RoleName) 
                    ? $"¡Hubo un error al intentar actualizar al {singularTypeName}!" 
                    : "¡Hubo un error al intentar actualizar a una persona con cargo!",
                Object = null
            };
        }
    }
}