using System.Security.Claims;
using DTO.DTOs.DTO_Response;

namespace Repositorio.Repository;

public interface ITokenService
{
    public string GenerateAccessToken(IEnumerable<Claim> claims);
    public string GenerateRefreshToken();
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}