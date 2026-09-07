using System.Net;
using BD;
using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.StudentsDTO;
using DTO.ENUM;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.Students;
using File = BD.Entidades.File;

namespace Repositorio.Repository;

public class FileRepository : IFileRepository
{
    private readonly AppDbContext bbdd;

    public FileRepository(AppDbContext bbdd)
    {
        this.bbdd = bbdd;
    }

    public async Task<ResponseDTO<List<FileDTO>>> getFilesByStudentId(long studentId)
    {
        try
        {
            var filesList = await bbdd.FileCurriculum.Include(fc => fc.File)
                .Include(fc => fc.Curriculum)
                .Where(fc => fc.File.StudentId == studentId).AsTracking().Select(fc => new FileDTO()
                {
                    Id = fc.Id,
                    Code = fc.File.Code,
                    Career = fc.Curriculum.Career!.Name,
                    Curriculum = fc.Curriculum.Resolution,
                    CurriculumId = fc.CurriculumId,
                    Status = fc.File.Status.ToString()
                }).ToListAsync();

            return new ResponseDTO<List<FileDTO>>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = filesList
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al intentar obtener el listado de legajos del estudiante: " + e.Message);
            return new ResponseDTO<List<FileDTO>>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar obtener el listado de legajos del estudiante!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<FileReadDTO>> getFileById(long fileId)
    {
        try
        {
            var fileDTO = await bbdd.FileCurriculum.Include(fc => fc.File)
                .Include(fc => fc.Curriculum) .ThenInclude(fc => fc.Career)
                .AsTracking().FirstOrDefaultAsync(f => f.Id == fileId);

            if (fileDTO == null) throw new Exception("FileNotFound");
            
            return new ResponseDTO<FileReadDTO>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = new FileReadDTO()
                {
                    Id = fileDTO.Id,
                    Code = fileDTO.File.Code,
                    CareerId = fileDTO.Curriculum.Career.Id,
                    CurriculumId = fileDTO.CurriculumId,
                    Status = fileDTO.File.Status
                }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al intentar obtener el listado de títulos/certificados: " + e.Message);
            
            if(e.Message.Contains("FileNotFound"))
                return new ResponseDTO<FileReadDTO>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "¡El legajo solicitado no existe!",
                    Object = null
                };
            
            return new ResponseDTO<FileReadDTO>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = $"¡Hubo un error al intentar obtener el legajo del estudiante!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> createFile(FilePostDTO filePostDto)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            if (await bbdd.Files.AnyAsync(f => f.Code == filePostDto.Code)) throw new Exception("DuplicatedCode");

            File file = new File()
            {
                Code = filePostDto.Code,
                StudentId = filePostDto.StudentId,
                Status = filePostDto.Status,
                CreatedBy = filePostDto.CreatedById ?? Guid.Empty
            };
            bbdd.Files.Add(file);
            await bbdd.SaveChangesAsync();

            bbdd.FileCurriculum.Add(new FileCurriculum()
            {
                CurriculumId = filePostDto.CurriculumId,
                FileId = file.Id,
                CreatedBy = filePostDto.CreatedById ?? Guid.Empty,
            });

            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Legajo creado con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error al intentar crear un legajo: " + e.Message);

            if (e.Message.Contains("DuplicatedCode"))
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "¡Ya existe un legajo con ese código, pruebe otro código!",
                    Object = null
                };

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar crear el legajo!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> updateFile(FilePostDTO filePostDto)
    {
        using var transaction = await bbdd.Database.BeginTransactionAsync();
        try
        {
            if (await bbdd.FileCurriculum.AnyAsync(f => f.File.Code == filePostDto.Code && f.Id != filePostDto.Id)) throw new Exception("DuplicatedCode");

            var file = await bbdd.FileCurriculum.Include(fc => fc.File).
                FirstOrDefaultAsync(f => f.Id == filePostDto.Id);
            
            if(file == null) throw new Exception("FileNotFound");
            
            file.File.Code = filePostDto.Code;
            file.File.UpdatedBy = filePostDto.UpdatedById ?? Guid.Empty;
            file.UpdatedBy = filePostDto.UpdatedById ?? Guid.Empty;
            file.CurriculumId = filePostDto.CurriculumId;
            file.File.Status = filePostDto.Status;
            
            await bbdd.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Operación éxitosa!",
                Object = "¡Legajo actualizado con éxito!"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine("Error al intentar actualizar un legajo: " + e.Message);
            Console.WriteLine(e.StackTrace);

            if (e.Message.Contains("DuplicatedCode"))
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "¡Ya existe un legajo con ese código, pruebe otro código!",
                    Object = null
                }; 
            if (e.Message.Contains("FileNotFound"))
                return new ResponseDTO<string>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "¡El legajo que intenta actualizar no existe!",
                    Object = null
                };

            return new ResponseDTO<string>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar actualizar el legajo!",
                Object = null
            };
        }
    }
}