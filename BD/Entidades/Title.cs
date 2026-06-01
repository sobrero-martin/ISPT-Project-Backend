namespace BD.Entidades;

public class Title : BaseEntity
{
    public long PersonId { get; set; }
    public Person Person { get; set; }
    
    public string TitleName { get; set; }
}