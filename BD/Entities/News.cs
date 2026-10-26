using System.ComponentModel.DataAnnotations;

namespace BD.Entidades;

public class News : BaseEntity
{
    [Required, MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Content { get; set; } = string.Empty;
}
