namespace DTO.DTOs.Documentation;

public class AddDeliverableDocYearDTO
{
    public Guid? CreatedById { get; set; }
    public long FileId { get; set; }
    public int Year { get; set; }
}