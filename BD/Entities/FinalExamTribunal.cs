using BD.Entidades;
using DTO.ENUM;
using Microsoft.EntityFrameworkCore;

namespace BD.Entities;

[Index(nameof(PersonId), nameof(FinalExamId), IsUnique = true)]
public class FinalExamTribunal : BaseEntity
{
    public long PersonId { get; set; }
    public Person Person { get; set; }
    
    public long FinalExamId { get; set; }
    public FinalExam FinalExam { get; set; }
    
    public EnumFinalExamTribunalRole FinalExamTribunalRole { get; set; }
}