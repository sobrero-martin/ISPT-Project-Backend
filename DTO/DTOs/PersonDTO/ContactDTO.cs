namespace DTO.DTOs.PersonDTO;

public class ContactDTO
{
    public long? ContactId { get; set; }
    public Guid? UpdatedById { get; set; }

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
        
    public string? EmergencyNumber { get; set; }
    public string? ContactNameEmergency { get; set; }
}