using Microsoft.EntityFrameworkCore;

namespace BD.Entidades;

[Index(nameof(FileId), nameof(CurriculumId), IsUnique = true)]
public class FileCurriculum : BaseEntity
{
    public long FileId { get; set; }
    public File File { get; set; }
    
    public long CurriculumId { get; set; }
    public Curriculum Curriculum { get; set; }
}