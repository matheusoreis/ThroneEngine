using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Throne.Server.Models;
using Throne.Server.Services;

namespace Throne.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(ITokenService tokenService, IUserService userService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] SignUp model)
    {
        bool result = await userService.SignUp(model.Username ?? "a", model.Password ?? "b");
        if (result) return Ok("User registered successfully.");
        return BadRequest("User already exists.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] SignIn model)
    {
        string? token = await tokenService.GenerateToken(model);

        return Ok(token);

        // if (token.IsNullOrEmpty()) return Unauthorized();
        //
        // bool result = await userService.SignIn(model.Email ?? "a", model.Password ?? "b");
        // if (result) return Ok(new { Token = token });
        //
        // return Unauthorized("Invalid credentials.");
    }

    [HttpGet("secure-data")]
    [Authorize]
    public IActionResult GetSecureData()
    {
        return Ok(new { Message = "This is protected data." });
    }
}