using DTO.DTOs.AuthDTO;
using Microsoft.AspNetCore.Mvc;
using Repositorio.Repository;

namespace ISPT_Project_Backend.Server.Controllers;

[ApiController]
[Route("api-v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        this.authRepository = authRepository;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthDTO authDTO)
    {
        var res = await authRepository.Login(authDTO);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(TokenDTO tokenDTO)
    {
        var res = await authRepository.Refresh(tokenDTO);
        return StatusCode((int)res.StatusCode, res);
    }
}