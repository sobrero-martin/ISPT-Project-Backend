using DTO.DTOs.DTO_Response;
using DTO.DTOs.NewsDTO;

namespace Repositorio.Implementations.News;

public interface INewsRepository
{
    Task<ResponseDTO<List<NewsDTO>>> GetAll();
    Task<ResponseDTO<NewsDTO>> Create(NewsPostDTO newsPostDTO, Guid createdBy);
    Task<ResponseDTO<string>> Delete(long id, Guid deletedBy);
}
