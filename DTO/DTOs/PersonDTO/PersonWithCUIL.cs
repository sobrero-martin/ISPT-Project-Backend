using System.ComponentModel.DataAnnotations;

namespace DTO.DTOs.PersonDTO;

public class PersonWithCUIL
{
    [Required(AllowEmptyStrings = false)]
    public string CUIL  { get; set; }
    public string? RoleName { get; set; }
}