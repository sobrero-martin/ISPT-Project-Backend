namespace DTO.DTOs.AuthDTO;

public class AuthDTO
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } =  string.Empty;
    public bool RememberMe { get; set; } =  false;
}