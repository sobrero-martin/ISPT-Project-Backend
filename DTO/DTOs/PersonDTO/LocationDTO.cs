using System.ComponentModel.DataAnnotations;

namespace DTO.DTOs.PersonDTO;

public class LocationDTO
{
    [Required(AllowEmptyStrings = false)]
    public required string Country { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public required string Province { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public required string Department { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public required string Address { get; set; }
}