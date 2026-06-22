namespace DTO.DTOs.PersonDTO;

public class ObservationDTO
{
    public long? PersonId { get; set; }
    public Guid? UpdatedById { get; set; }
    public string? Observation { get; set; }
}