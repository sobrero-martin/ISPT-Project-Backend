namespace DTO.DTOs.CareerDTO;

public class SubjectTableDTO
{
    public long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public int Year { get; set; }
    public required string Format { get; set; }
}