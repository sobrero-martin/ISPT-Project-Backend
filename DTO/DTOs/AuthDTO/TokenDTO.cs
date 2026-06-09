namespace DTO.DTOs.AuthDTO;

public class TokenDTO
{
    public string AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
}