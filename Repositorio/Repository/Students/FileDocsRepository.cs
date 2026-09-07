using System.Net;
using BD;
using BD.Entidades;
using DTO.DTOs.Documentation;
using DTO.DTOs.DTO_Response;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Students;

namespace Repositorio.Repository;

public class FileDocsRepository : IFileDocsRepository
{
    private readonly AppDbContext bbdd;

    public FileDocsRepository(AppDbContext bbdd)
    {
        this.bbdd = bbdd;
    }

    // Devolver los años ordenados del mayor al menor
    public async Task<ResponseDTO<FileDocumentation>> GetAllDocumentsByFileId(long fileId)
    {
        try
        {
            var file = await bbdd.Files.FirstOrDefaultAsync(f => f.Id == fileId);
            if (file == null) throw new Exception("FileNotFound");

            return new ResponseDTO<FileDocumentation>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = new FileDocumentation()
                {
                    FileId = file.Id,
                    DNI = file.DNI,
                    Picture = file.Picture,
                    BirthdateDocument = file.BirthdateDocument,
                    Rows = await bbdd.Documentations.Where(d => d.FileId == file.Id && d.state == true)
                        .OrderByDescending(d => d.Date)
                        .Select(d => new FileDocumentationRowPerYear()
                        {
                            Id = d.Id,
                            FileId = file.Id,
                            Date = d.Date,
                            CDA = d.CDA,
                            CNIRDS = d.CNIRDS,
                            Cooperative = d.Cooperative,
                            CUS = d.CUS,
                        }).ToListAsync()
                }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al intentar obtener documentación del legajo: " + e.Message);

            if (e.Message == "FileNotFound")
                return new ResponseDTO<FileDocumentation>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "¡No se encontró documentación para ese legajo, el mismo no existe!",
                    Object = null
                };

            return new ResponseDTO<FileDocumentation>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar obtener la documentación del legajo!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<List<FileDocumentationRowPerYear>>> ReloadDocumentsDeliverableByYear(long fileId)
    {
        try
        {
            return new ResponseDTO<List<FileDocumentationRowPerYear>>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = await bbdd.Documentations.Where(d => d.FileId == fileId && d.state == true)
                    .OrderByDescending(d => d.Date)
                    .Select(d => new FileDocumentationRowPerYear()
                    {
                        Id = d.Id,
                        FileId = fileId,
                        Date = d.Date,
                        CDA = d.CDA,
                        CNIRDS = d.CNIRDS,
                        Cooperative = d.Cooperative,
                        CUS = d.CUS,
                    }).ToListAsync()
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al intentar obtener documentación del legajo: " + e.Message);

            return new ResponseDTO<List<FileDocumentationRowPerYear>>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar obtener la documentación anual del legajo!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> ChangeDocumentsStatus(FileDocumentation documentation)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var file = await bbdd.Files.FirstOrDefaultAsync(d => d.Id == documentation.FileId);

            if (file == null) throw new Exception("FileNotFound");

            file.DNI = documentation.DNI;
            file.BirthdateDocument = documentation.BirthdateDocument;
            file.Picture = documentation.Picture;
            file.UpdatedBy = documentation.UpdatedById;

            if (documentation.Rows.Any())
            {
                var rowIds = documentation.Rows.Select(r => r.Id).ToList();
                
                var existingDocs = await bbdd.Documentations
                    .Where(d => rowIds.Contains(d.Id) && d.FileId == documentation.FileId)
                    .ToDictionaryAsync(d => d.Id);
                
                foreach (var row in documentation.Rows)
                {
                    if (existingDocs.TryGetValue(row.Id, out var entity))
                    {
                        entity.CUS = row.CUS;
                        entity.CNIRDS = row.CNIRDS;
                        entity.CDA = row.CDA;
                        entity.Cooperative = row.Cooperative;
                        entity.UpdatedBy = row.UpdatedById;
                    }
                }
            }

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Documentación del legajo actualizada con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error al intentar actualizar la documentación del legajo: " + e.Message);

            if (e.Message.Contains("FileNotFound"))
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "¡No existe documentación para este legajo, el legajo no existe!",
                    Object = null
                };

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar actualizar la documentación del legajo!",
                Object = null
            };
        }
    }

    // Si ya se dio la baja lógica previamente, se vuelve a activar nomás
    public async Task<ResponseDTO<string>> AddYearInDeliverableDocumentsPerYear(AddDeliverableDocYearDTO DTO)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var rowDocument =
                await bbdd.Documentations.FirstOrDefaultAsync(d => d.FileId == DTO.FileId && d.Date == DTO.Year);

            switch (rowDocument)
            {
                case { state: true }:
                    throw new Exception("RowExistent");
                case { state: false }:
                    rowDocument.state = true;
                    break;
                default:
                    bbdd.Documentations.Add(new Documentation
                    {
                        CreatedBy = DTO.CreatedById ?? Guid.Empty,
                        FileId = DTO.FileId,
                        Date = DTO.Year,
                        CDA = false,
                        CNIRDS = false,
                        Cooperative = false,
                        CUS = false
                    });
                    break;
            }

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Año añadido con éxito a la tabla!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error al intentar añadir un año a la tabla de documentación: " + e.Message);

            if (e.Message.Contains("RowExistent"))
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "¡Ya existe ese año en la tabla de documentación anual!",
                    Object = null
                };

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar añadir el año a la tabla de documentación anual!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> DeleteYearInDeliverableDocumentsPerYear(long id)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            var rowDocument =
                await bbdd.Documentations.FirstOrDefaultAsync(d => d.Id == id);

            if (rowDocument == null) throw new Exception("RowNotExist");

            rowDocument.state = false;
            rowDocument.CUS = false;
            rowDocument.CDA = false;
            rowDocument.CNIRDS = false;
            rowDocument.Cooperative = false;

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Año eliminado con éxito de la tabla!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error al intentar eliminar un año de la tabla de documentación: " + e.Message);

            if (e.Message.Contains("RowNotExist"))
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "¡El año de la tabla de documentación anual que se desea eliminar no existe!",
                    Object = null
                };

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar eliminar el año a la tabla de documentación anual!",
                Object = null
            };
        }
    }
}