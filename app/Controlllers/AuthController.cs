using Microsoft.AspNetCore.Mvc;
using MiApi.DTOs;
using MiApi.Services;

namespace MiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var tokenResponse = await _authService.Login(request);
        if (tokenResponse == null)
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

        return Ok(tokenResponse);
    }
}