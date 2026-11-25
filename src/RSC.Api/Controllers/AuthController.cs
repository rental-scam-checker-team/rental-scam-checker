using Microsoft.AspNetCore.Mvc;
using RSC.Classes;

namespace RSC.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AdminCollection _admins = new();

    [HttpPost("login")]
    public IActionResult Login(LoginInput input)
    {
        var admin = _admins.Login(input.Username ?? "", input.Password ?? "");
        if (admin == null)
            return Unauthorized(new { error = "Wrong username or password." });

        return Ok(new { adminId = admin.AdminId, username = admin.Username });
    }
}

public class LoginInput
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
