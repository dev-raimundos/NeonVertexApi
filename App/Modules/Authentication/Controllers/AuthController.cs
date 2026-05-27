using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeonVertexApi.App.Modules.Authentication.DTOs;
using NeonVertexApi.App.Modules.Authentication.Services;
using NeonVertexApi.App.Shared.Interfaces;

namespace NeonVertexApi.App.Modules.Authentication.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(AuthService service, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (response, token) = await service.LoginAsync(dto);

        Response.Cookies.Append("access_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(24)
        });

        return Ok(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(new
        {
            currentUser.Id,
            currentUser.Email,
            currentUser.Name
        });
    }
}