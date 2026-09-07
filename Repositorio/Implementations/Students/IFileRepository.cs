using DTO.DTOs.DTO_Response;
using DTO.DTOs.StudentsDTO;

namespace Repositorio.Implementations.Students;

public interface IFileRepository
{
    Task<ResponseDTO<List<FileDTO>>> getFilesByStudentId(long studentId);
    Task<ResponseDTO<FileReadDTO>> getFileById(long fileId);
    Task<ResponseDTO<string>> createFile(FilePostDTO filePostDto);
    Task<ResponseDTO<string>> updateFile(FilePostDTO filePutDto);
}