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
        var records = await context.News
            .AsNoTracking()
            .Where(item => item.state)
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => new
            {
                Id = item.Id,
                Title = item.Title,
                Content = item.Content,
                CreatedAt = item.CreatedAt,
                item.CreatedBy,
                AuthorName = item.AuthorPerson == null
                    ? null
                    : $"{item.AuthorPerson.Firstname} {item.AuthorPerson.Lastname}"
            })
            .ToListAsync();

        var unmatchedUserIds = records
            .Where(item => item.AuthorName == null)
            .Select(item => item.CreatedBy.ToString())
            .Distinct()
            .ToList();
        var usernames = await context.Users
            .Where(user => unmatchedUserIds.Contains(user.Id))
            .Select(user => new { user.Id, user.UserName })
            .ToDictionaryAsync(user => user.Id, user => user.UserName);

        var news = records.Select(item => new NewsDTO
        {
            Id = item.Id,
            Title = item.Title,
            Content = item.Content,
            CreatedAt = item.CreatedAt,
            AuthorName = item.AuthorName ?? usernames.GetValueOrDefault(item.CreatedBy.ToString())
        }).ToList();

        return new ResponseDTO<List<NewsDTO>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Novedades obtenidas exitosamente.",
            Object = news
        };
    }

    public async Task<ResponseDTO<NewsDTO>> Create(NewsPostDTO newsPostDTO, Guid createdBy, long? authorPersonId)
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
            CreatedBy = createdBy,
            AuthorPersonId = authorPersonId
        };

        if (authorPersonId.HasValue)
            news.AuthorPerson = await context.People.FindAsync(authorPersonId.Value);
        context.News.Add(news);
        await context.SaveChangesAsync();

        var authorName = news.AuthorPerson == null
            ? await context.Users.Where(user => user.Id == createdBy.ToString()).Select(user => user.UserName).FirstOrDefaultAsync()
            : $"{news.AuthorPerson.Firstname} {news.AuthorPerson.Lastname}";

        return new ResponseDTO<NewsDTO>
        {
            StatusCode = HttpStatusCode.Created,
            Message = "Novedad publicada exitosamente.",
            Object = new NewsDTO
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                CreatedAt = news.CreatedAt,
                AuthorName = authorName
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
