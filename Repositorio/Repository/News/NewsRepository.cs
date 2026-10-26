using System.Net;
using BD;
using BD.Entidades;
using DTO.DTOs.DTO_Response;
using DTO.DTOs.NewsDTO;
using Microsoft.EntityFrameworkCore;
using Repositorio.Implementations.News;

namespace Repositorio.Repository.News;

public class NewsRepository : INewsRepository
{
    private readonly AppDbContext context;

    public NewsRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<ResponseDTO<List<NewsDTO>>> GetAll()
    {
        var news = await context.News
            .AsNoTracking()
            .Where(item => item.state)
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => new NewsDTO
            {
                Id = item.Id,
                Title = item.Title,
                Content = item.Content,
                CreatedAt = item.CreatedAt
            })
            .ToListAsync();

        return new ResponseDTO<List<NewsDTO>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Novedades obtenidas exitosamente.",
            Object = news
        };
    }

    public async Task<ResponseDTO<NewsDTO>> Create(NewsPostDTO newsPostDTO, Guid createdBy)
    {
        var title = newsPostDTO.Title.Trim();
        var content = newsPostDTO.Content.Trim();
        if (title.Length < 3 || content.Length < 3)
        {
            return new ResponseDTO<NewsDTO>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "El título y el texto deben tener al menos 3 caracteres.",
                Object = null
            };
        }

        var news = new BD.Entidades.News
        {
            Title = title,
            Content = content,
            CreatedBy = createdBy
        };

        context.News.Add(news);
        await context.SaveChangesAsync();

        return new ResponseDTO<NewsDTO>
        {
            StatusCode = HttpStatusCode.Created,
            Message = "Novedad publicada exitosamente.",
            Object = new NewsDTO
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                CreatedAt = news.CreatedAt
            }
        };
    }

    public async Task<ResponseDTO<string>> Delete(long id, Guid deletedBy)
    {
        var news = await context.News.FirstOrDefaultAsync(item => item.Id == id && item.state);
        if (news == null)
        {
            return new ResponseDTO<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "Novedad no encontrada.",
                Object = null
            };
        }

        news.state = false;
        news.UpdatedBy = deletedBy;
        await context.SaveChangesAsync();

        return new ResponseDTO<string>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Novedad eliminada exitosamente.",
            Object = "Novedad eliminada exitosamente."
        };
    }
}
