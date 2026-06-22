using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DTO.DTOs.DTO_Response;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Repositorio.Repository;

public class TokenService : ITokenService
{
    private readonly IConfiguration configuration;
    private static readonly JwtSecurityTokenHandler tokenHandler = new();

    public TokenService(IConfiguration configuration)
    {
        this.configuration = configuration;
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
    }
    
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        try
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMinutes(15),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return tokenHandler.WriteToken(token);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al generar el access token: " + e.Message);
            return string.Empty;
        }
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
            ValidateLifetime = false,
            NameClaimType = "Username",
            RoleClaimType = "Role"
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Token inválido");

        return principal;
    }
}