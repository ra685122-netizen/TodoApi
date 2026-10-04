using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthenticationController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var result = _authService.Login(request);

        if (result is null)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("token/refresh")]
    public IActionResult RefreshToken(RefreshTokenRequest request)
    {
        var result = _authService.RefreshToken(request.RefreshToken);

        if (result is null)
        {
            return Unauthorized("Invalid or expired refresh token.");
        }

        return Ok(result);
    }
}