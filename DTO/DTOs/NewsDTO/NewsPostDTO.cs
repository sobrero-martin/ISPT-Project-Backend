using System.ComponentModel.DataAnnotations;

namespace DTO.DTOs.NewsDTO;

public class NewsPostDTO
{
    [Required, StringLength(120, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(4000, MinimumLength = 3)]
    public string Content { get; set; } = string.Empty;
}
