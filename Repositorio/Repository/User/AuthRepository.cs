using System.Net;
using System.Security.Claims;
using BD;
using DTO.DTOs.AuthDTO;
using DTO.DTOs.DTO_Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Repositorio.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly ITokenService tokenService;
    private readonly AppDbContext appDbContext;

    public AuthRepository(UserManager<IdentityUser> userManager, ITokenService tokenService, AppDbContext appDbContext)
    {
        this.userManager = userManager;
        this.tokenService = tokenService;
        this.appDbContext = appDbContext;
    }

    public async Task<ResponseDTO<TokenDTO>> Login(AuthDTO authDTO)
    {
        try
        {
            var user = await userManager.FindByNameAsync(authDTO.Username);
            if (user == null)
                return new ResponseDTO<TokenDTO>()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "Usuario o contraseña incorrectos.",
                    Object = null
                };

            var isPasswordValid = await userManager.CheckPasswordAsync(user, authDTO.Password);
            if (!isPasswordValid)
                return new ResponseDTO<TokenDTO>()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "Usuario o contraseña incorrectos.",
                    Object = null
                };

            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim("ID", user.Id),
                new Claim("Username", user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim("Role", role));
            }

            var person = await appDbContext.People.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (person != null)
            {
                authClaims.Add(new Claim("PersonId", person.Id.ToString()));
                authClaims.Add(new Claim("Fullname", $"{person.Firstname} {person.Lastname}"));
            }

            var accessToken = tokenService.GenerateAccessToken(authClaims);
            string? refreshToken = null;

            if (authDTO.RememberMe)
            {
                refreshToken = tokenService.GenerateRefreshToken();
                await userManager.SetAuthenticationTokenAsync(user, "JWTAuth", "RefreshToken", refreshToken);
            }
            else
            {
                await userManager.RemoveAuthenticationTokenAsync(user, "JWTAuth", "RefreshToken");
            }

            return new ResponseDTO<TokenDTO>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Token generado con éxito!",
                Object = new TokenDTO()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = 900
                }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error en el login:" + e.Message);
            return new ResponseDTO<TokenDTO>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "¡Hubo un error al intentar iniciar sesión!",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<TokenDTO>> Refresh(TokenDTO tokenDTO)
    {
        if (tokenDTO == null || string.IsNullOrEmpty(tokenDTO.AccessToken) ||
            string.IsNullOrEmpty(tokenDTO.RefreshToken))
            return new ResponseDTO<TokenDTO>()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Tokens inválidos.",
                Object = null
            };

        try
        {
            var principal = tokenService.GetPrincipalFromExpiredToken(tokenDTO.AccessToken);
            var username = principal?.FindFirst("Username")?.Value;

            if (username == null)
                return new ResponseDTO<TokenDTO>()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Access Token inválido.",
                    Object = null
                };

            var user = await userManager.FindByNameAsync(username);
            if (user == null)
                return new ResponseDTO<TokenDTO>()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Usuario no encontrado.",
                    Object = null
                };

            var savedRefreshToken = await userManager.GetAuthenticationTokenAsync(user, "JWTAuth", "RefreshToken");
            if (savedRefreshToken != tokenDTO.RefreshToken)
                return new ResponseDTO<TokenDTO>()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "Refresh Token inválido o expirado.",
                    Object = null
                };

            var tokenMetadataClaims = new HashSet<string>
            {
                JwtRegisteredClaimNames.Exp,
                JwtRegisteredClaimNames.Iat,
                JwtRegisteredClaimNames.Nbf,
                JwtRegisteredClaimNames.Iss,
                JwtRegisteredClaimNames.Aud,
                JwtRegisteredClaimNames.Jti
            };
            var refreshedClaims = principal!.Claims
                .Where(claim => !tokenMetadataClaims.Contains(claim.Type))
                .ToList();
            refreshedClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var newAccessToken = tokenService.GenerateAccessToken(refreshedClaims);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            await userManager.SetAuthenticationTokenAsync(user, "JWTAuth", "RefreshToken", newRefreshToken);

            return new ResponseDTO<TokenDTO>()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "¡Token renovado con éxito!",
                Object = new TokenDTO()
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresIn = 900
                }
            };
        }
        catch (Exception)
        {
            Console.WriteLine();
            return new ResponseDTO<TokenDTO>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Error interno al intentar actualizar token.",
                Object = null
            };
        }
    }

    public async Task<ResponseDTO<string>> Logout(TokenDTO tokenDTO)
    {
        try
        {
            if (tokenDTO != null && !string.IsNullOrEmpty(tokenDTO.AccessToken)
                && !string.IsNullOrEmpty(tokenDTO.RefreshToken))
            {
                var principal = tokenService.GetPrincipalFromExpiredToken(tokenDTO.AccessToken);
                var username = principal?.FindFirst("Username")?.Value;
                if (!string.IsNullOrEmpty(username))
                {
                    var user = await userManager.FindByNameAsync(username);
                    var savedRefreshToken = user == null
                        ? null
                        : await userManager.GetAuthenticationTokenAsync(user, "JWTAuth", "RefreshToken");

                    if (user != null && savedRefreshToken == tokenDTO.RefreshToken)
                        await userManager.RemoveAuthenticationTokenAsync(user, "JWTAuth", "RefreshToken");
                }
            }

            return new ResponseDTO<string>
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Sesión cerrada.",
                Object = "Sesión cerrada."
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al cerrar la sesión: " + e.Message);
            return new ResponseDTO<string>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "No se pudo invalidar la sesión en el servidor.",
                Object = null
            };
        }
    }
}
