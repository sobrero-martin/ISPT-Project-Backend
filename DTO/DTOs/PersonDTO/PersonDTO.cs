using System.ComponentModel.DataAnnotations;

namespace DTO.DTOs.PersonDTO;

public class PersonDTO
{
    public long? Id { get; set; }
    public Guid? CreatedById { get; set; }
    public Guid? UpdatedById { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public required string Firstname { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public required string Lastname { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public required string Gender { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public required string CUIL { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public required string TypeDocument  { get; set; }
    
    [Required(AllowEmptyStrings = false)]
    public required string DocumentNumber { get; set; }
        
    public required DateTime Birthdate { get; set; }
    public string? PracticePlace { get; set; }
    public string? Observations { get; set; }
        
    public ContactDTO? ContactDTO { get; set; }
    public required LocationDTO LocationDTO { get; set; }
    
    // Only accepted by users with role "Directivo"
    public string? RoleName { get; set; }
}