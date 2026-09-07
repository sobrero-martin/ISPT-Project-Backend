using DTO.DTOs.Documentation;
using DTO.DTOs.DTO_Response;

namespace Repositorio.Implementations.Students;

public interface IFileDocsRepository
{
    Task<ResponseDTO<FileDocumentation>> GetAllDocumentsByFileId(long fileId);
    Task<ResponseDTO<List<FileDocumentationRowPerYear>>> ReloadDocumentsDeliverableByYear(long fileId);
    Task<ResponseDTO<string>> ChangeDocumentsStatus(FileDocumentation documentation);
    Task<ResponseDTO<string>> AddYearInDeliverableDocumentsPerYear(AddDeliverableDocYearDTO DTO);
    Task<ResponseDTO<string>> DeleteYearInDeliverableDocumentsPerYear(long id);
}