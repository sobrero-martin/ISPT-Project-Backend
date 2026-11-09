using DTO.DTOs.DTO_Response;
using DTO.DTOs.NewsDTO;
using BD;
using Microsoft.EntityFrameworkCore;
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
    private readonly AppDbContext dbContext;

    public NewsController(INewsRepository newsRepository, AppDbContext dbContext)
    {
        this.newsRepository = newsRepository;
        this.dbContext = dbContext;
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

        var authorPersonId = await dbContext.People
            .Where(person => person.UserId == userId)
            .Select(person => (long?)person.Id)
            .FirstOrDefaultAsync();

        var response = await newsRepository.Create(newsPostDTO, createdBy, authorPersonId);
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
