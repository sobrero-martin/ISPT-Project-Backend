using System.Linq.Expressions;
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
    private readonly string singularTypeNameFirstLetterUpper;

    // ONLY ONE ROL
    public PersonRepository(AppDbContext bbdd, UserManager<IdentityUser> userManager,
        string roleName, string singularTypeName, string pluralTypeName)
    {
        this.bbdd = bbdd;
        this.userManager = userManager;
        roleNames = new List<string>() { roleName };
        this.singularTypeName = singularTypeName;
        this.pluralTypeName = pluralTypeName;
        singularTypeNameFirstLetterUpper = char.ToUpper(singularTypeName[0]) + singularTypeName.Substring(1);
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

    public async Task<ResponseDTO<PersonDTO>> GetPerson(long id)
    {
        try
        {
            var p = await bbdd.People.AsNoTracking()
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

    public async Task<ResponseDTO<ContactDTO>> GetContactByPersonId(long id)
    {
        try
        {
            var c = await bbdd.Contacts.AsNoTracking()
                .FirstOrDefaultAsync(p => p.PersonId == id);

            if (c == null) throw new Exception($"No hay un {singularTypeName} con ese ID.");

            return new ResponseDTO<ContactDTO>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = new ContactDTO()
                {
                    ContactId = c.Id,
                    ContactNameEmergency = c.ContactNameEmergency,
                    EmergencyNumber = c.EmergencyNumber,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email,
                }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al intentar obtener el contacto de {singularTypeName}: " + e.Message);
            return new ResponseDTO<ContactDTO>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"¡Hubo un error al intentar obtener el contacto del {singularTypeName}!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<ObservationDTO>> GetObservationByPersonId(long id)
    {
        try
        {
            var p = await bbdd.People.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) throw new Exception($"No hay un {singularTypeName} con ese ID.");

            return new ResponseDTO<ObservationDTO>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = new ObservationDTO()
                {
                    PersonId = p.Id,
                    Observation = p.Observations
                }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al intentar obtener la observación de {singularTypeName}: " + e.Message);
            return new ResponseDTO<ObservationDTO>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"¡Hubo un error al intentar obtener la observación del {singularTypeName}!",
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
            if (!resultUsuario.Succeeded)
            {
                if (resultUsuario.Errors.Any(x => x.Code == "DuplicateUserName"))
                {
                    throw new Exception("DuplicateUserName");
                }

                var errores = string.Join(", ", resultUsuario.Errors.Select(e => e.Description));
                throw new Exception($"Error de Identity: {errores}");
            }

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
            bbdd.People.Add(p);
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
                Object = $"¡{singularTypeNameFirstLetterUpper} creado con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error al intentar crear al {singularTypeName}: " + e.Message + $", {e.ToString()}");

            if (e.Message == "DuplicateUserName" ||
                (e.InnerException != null && (e.InnerException.Message.Contains("duplicate") ||
                                              e.InnerException.Message.Contains("IX_"))))
            {
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "¡El CUIL ingresado ya existe en el sistema!",
                    Object = null
                };
            }

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"¡Hubo un error al intentar crear al {singularTypeName}!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> AddRoleToPerson(PersonWithCUIL personWithCUIL)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var user = await userManager.FindByNameAsync(personWithCUIL.CUIL);
            if (user == null)
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "¡No existe una persona con ese CUIL en el sistema",
                    Object = null
                };

            if (string.IsNullOrEmpty(personWithCUIL.RoleName))
            {
                var addResult = await userManager.AddToRoleAsync(user, roleNames.First());
                if (!addResult.Succeeded) throw new Exception($"Error al asignar el rol de: {roleNames.First()}");
            }
            else
            {
                var currentRoles = await userManager.GetRolesAsync(user);
                var rolesToRemove = currentRoles.Intersect(roleNames).ToList();
                
                if (rolesToRemove.Any())
                {
                     var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
                     if (!removeResult.Succeeded) throw new Exception("Error al borrar los roles del usuario.");
                }
                
                var addResult = await userManager.AddToRoleAsync(user, personWithCUIL.RoleName);
                if (!addResult.Succeeded) throw new Exception($"Error al asignar el nuevo rol: {personWithCUIL.RoleName}");
            }

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = string.IsNullOrEmpty(personWithCUIL.RoleName)
                    ? $"¡Rol de {singularTypeName} asignado con éxito en la persona!"
                    : "¡Rol de cargo asignado con éxito en la persona!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error al intentar asignarle un nuevo rol a la persona: " + e.Message);

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = string.IsNullOrEmpty(personWithCUIL.RoleName)
                    ? $"¡Hubo un error al intentar actualizar al {singularTypeName}!"
                    : "¡Hubo un error al intentar actualizar a una persona con cargo!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> EditPerson(PersonDTO personDTO)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var person = await bbdd.People.FindAsync(personDTO.Id);
            if (person == null) throw new Exception("La persona que se desea actualizar no existe.");

            var user = await userManager.FindByIdAsync(person.UserId);
            if (user == null) throw new Exception("La persona no cuenta con un usuario.");

            if (person.CUIL != personDTO.CUIL)
            {
                user.UserName = personDTO.CUIL;
                user.NormalizedUserName = personDTO.CUIL;

                var resultUsuario = await userManager.UpdateAsync(user);
                if (!resultUsuario.Succeeded)
                {
                    if (resultUsuario.Errors.Any(x => x.Code == "DuplicateUserName"))
                    {
                        throw new Exception("DuplicateUserName");
                    }

                    var errores = string.Join(", ", resultUsuario.Errors.Select(e => e.Description));
                    throw new Exception($"Error de Identity: {errores}");
                }
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
            person.UpdatedBy = personDTO.UpdatedById;

            var location = await bbdd.Locations.FirstAsync(l => l.PersonId == person.Id);
            location.Address = personDTO.LocationDTO.Address;
            location.Country = personDTO.LocationDTO.Country;
            location.Department = personDTO.LocationDTO.Department;
            location.Province = personDTO.LocationDTO.Province;
            location.UpdatedBy = personDTO.UpdatedById;

            var contact = await bbdd.Contacts.FirstAsync(c => c.PersonId == person.Id);
            contact.Email = personDTO.ContactDTO?.Email;
            contact.PhoneNumber = personDTO.ContactDTO?.PhoneNumber;
            contact.ContactNameEmergency = personDTO.ContactDTO?.ContactNameEmergency;
            contact.EmergencyNumber = personDTO.ContactDTO?.EmergencyNumber;
            contact.UpdatedBy = personDTO.UpdatedById;

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = string.IsNullOrEmpty(personDTO.RoleName)
                    ? $"¡{singularTypeNameFirstLetterUpper} actualizado con éxito!"
                    : "¡Persona con cargo actualizada con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error al intentar actualizar al {singularTypeName}: " + e.Message);

            if (e.Message == "DuplicateUserName" ||
                (e.InnerException != null && (e.InnerException.Message.Contains("duplicate") ||
                                              e.InnerException.Message.Contains("IX_"))))
            {
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "¡El CUIL ingresado ya existe en el sistema, no puede haber duplicados!",
                    Object = null
                };
            }

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

    public async Task<ResponseDTO<string>> EditContact(ContactDTO contactDTO)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var c = await bbdd.Contacts.FindAsync(contactDTO.ContactId);
            if (c == null) throw new Exception("El contacto que se desea actualizar no existe.");

            c.UpdatedBy = contactDTO.UpdatedById;
            c.Email = contactDTO.Email;
            c.PhoneNumber = contactDTO.PhoneNumber;
            c.EmergencyNumber = contactDTO.EmergencyNumber;
            c.ContactNameEmergency = contactDTO.ContactNameEmergency;

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Contacto actualizado con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error al intentar actualizar al {singularTypeName}: " + e.Message);
            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar actualizar el contacto!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> EditObservation(ObservationDTO observationDTO)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var p = await bbdd.People.FindAsync(observationDTO.PersonId);
            if (p == null)
                throw new Exception(
                    "La observación de la persona que se desea actualizar no se puede porque la persona no existe.");

            p.UpdatedBy = observationDTO.UpdatedById;
            p.Observations = observationDTO.Observation;

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Observación actualizada con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error al intentar actualizar la observación del {singularTypeName}: " + e.Message);
            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar actualizar la observación!",
                Object = null
            };
        }
    }
}