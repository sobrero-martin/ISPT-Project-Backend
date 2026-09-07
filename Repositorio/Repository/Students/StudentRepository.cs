using BD;
using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.SchoolYearDTO;
using DTO.DTOs.StudentsDTO;
using ExcelDataReader;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using Repositorio.Implementations.Students;
using File = BD.Entidades.File;


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

    public async Task<ResponseDTO<List<StudentFileDivisionDTO>>> GetStudentsBySchoolYearId(long schoolYearId)
    {

        try
        {
            var curriculum = await bbdd.Set<SchoolYear>()
                .AsNoTracking()
                .Where(sy => sy.Id == schoolYearId)
                .Select(sy => sy.Curriculum)
                .FirstOrDefaultAsync();

            if (curriculum == null)
            {
                return new ResponseDTO<List<StudentFileDivisionDTO>>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "¡No se encontró el año escolar especificado o no tiene un plan de estudio asignado!",
                    Object = null
                };
            }

            var fileCurriculums = await bbdd.Set<FileCurriculum>()
                .AsNoTracking()
                .Where(fc => fc.CurriculumId == curriculum!.Id)
                .Select(fc => fc.FileId)
                .ToListAsync();

            if (!fileCurriculums.Any())
            {
                return new ResponseDTO<List<StudentFileDivisionDTO>>()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "¡No hay estudiantes registrados para el año escolar indicado!",
                    Object = new List<StudentFileDivisionDTO>()
                };
            }
            /*
            var files = await bbdd.Set<File>()
                .AsNoTracking()
                .Where(f => fileCurriculums.Contains(f.Id))
                .ToListAsync();*/

            var students = await bbdd.Set<Person>()
                .Where(p => p.Files.Any(f => fileCurriculums.Contains(f.Id)))
                .Select(p => new StudentFileDivisionDTO()
                {
                    FileId = p.Files.FirstOrDefault(f => fileCurriculums.Contains(f.Id))!.Id,
                    FileCode = p.Files.FirstOrDefault(f => fileCurriculums.Contains(f.Id))!.Code,
                    FirstName = p.Firstname,
                    LastName = p.Lastname,
                    DocumentNumber = p.DocumentNumber
                })
                .ToListAsync();
            return new ResponseDTO<List<StudentFileDivisionDTO>>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Listado de estudiantes por año escolar obtenido con éxito!",
                Object = students
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error al intentar obtener el listado de estudiantes por año escolar: " + e.Message);
            return new ResponseDTO<List<StudentFileDivisionDTO>>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar obtener el listado de estudiantes por año escolar!",
                Object = null
            };
        }
    }


    public async Task<ResponseDTO<int>> ImportStudentsFromExcel(Stream fileStream)
    {
        try
        {
            var studentsToImport = new List<StudentImportDTO>();

            using (var reader = ExcelReaderFactory.CreateReader(fileStream))
            {
                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true 
                    }
                });

                DataTable table = result.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    var docNumber = row[0]?.ToString()?.Trim();
                    var lastName = row[2]?.ToString()?.Trim();
                    var firstName = row[3]?.ToString()?.Trim();

                    if (string.IsNullOrEmpty(docNumber)) continue;

                    studentsToImport.Add(new StudentImportDTO
                    {
                        DocumentNumber = docNumber,
                        LastName = lastName,
                        FirstName = firstName
                    });
                }
            }

            if (!studentsToImport.Any())
            {
                return new ResponseDTO<int>
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "El archivo Excel no contiene datos válidos para importar.",
                    Object = 0
                };
            }

            var incomingDocs = studentsToImport.Select(s => s.DocumentNumber).ToList();

            var existingDocs = await bbdd.People
                .Where(p => incomingDocs.Contains(p.DocumentNumber))
                .Select(p => p.DocumentNumber)
                .ToListAsync();

            var processedDocsInFile = new HashSet<string>();
            var newPersons = new List<Person>();

            foreach (var dto in studentsToImport)
            {
                bool existsInDb = existingDocs.Contains(dto.DocumentNumber);
                bool isDuplicateInFile = !processedDocsInFile.Add(dto.DocumentNumber);

                if (!existsInDb && !isDuplicateInFile)
                {
                    var user = new IdentityUser
                    {
                        UserName = dto.DocumentNumber,
                        Email = $"{dto.DocumentNumber}@escuela.com",
                        EmailConfirmed = true
                    };

                    var createResult = await userManager.CreateAsync(user, $"{dto.DocumentNumber}");

                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Estudiante");

                        newPersons.Add(new Person
                        {
                            UserId = user.Id, 
                            DocumentNumber = dto.DocumentNumber,
                            Lastname = dto.LastName,
                            Firstname = dto.FirstName,
                            CUIL = dto.DocumentNumber,
                            TypeDocument = "DNI",
                            Gender = "Otro",
                            Birthdate = DateTime.MinValue,
                            CreatedAt = DateTime.Now,
                            state = true,
                            Observations = string.Empty,
                            PracticePlace = string.Empty
                        });
                    }
                }
            }

            int importedCount = 0;
            if (newPersons.Any())
            {
                await bbdd.People.AddRangeAsync(newPersons);
                await bbdd.SaveChangesAsync();
                importedCount = newPersons.Count;
            }

            return new ResponseDTO<int>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = $"Se importaron {importedCount} estudiantes correctamente.",
                Object = importedCount
            };
        }
        catch (Exception ex)
        {
            return new ResponseDTO<int>
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = $"Error al procesar el archivo Excel: {ex.Message}",
                Object = 0
            };
        }
    }
}
