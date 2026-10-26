using DTO.DTOs.DTO_Response;
using DTO.DTOs.NewsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Implementations.News;

namespace ISPT_Project_Backend.Server.Controllers;

[ApiController]
[Route("api-v1/news")]
[Authorize]
public class NewsController : ControllerBase
{
    private readonly INewsRepository newsRepository;

    public NewsController(INewsRepository newsRepository)
    {
        this.newsRepository = newsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<ResponseDTO<List<NewsDTO>>>> GetAll()
    {
        var response = await newsRepository.GetAll();
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<ActionResult<ResponseDTO<NewsDTO>>> Create(NewsPostDTO newsPostDTO)
    {
        var userId = User.FindFirst("ID")?.Value;
        if (!Guid.TryParse(userId, out var createdBy))
            return Unauthorized();

        var response = await newsRepository.Create(newsPostDTO, createdBy);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = "Directivo,Preceptor")]
    public async Task<ActionResult<ResponseDTO<string>>> Delete(long id)
    {
        var userId = User.FindFirst("ID")?.Value;
        if (!Guid.TryParse(userId, out var deletedBy))
            return Unauthorized();

        var response = await newsRepository.Delete(id, deletedBy);
        return StatusCode((int)response.StatusCode, response);
    }
}
