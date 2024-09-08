using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Throne.Server.Models;
using Throne.Server.Services;

namespace Throne.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService tokenService;
        private readonly IUserService userService;

        public AuthController(ITokenService tokenService, IUserService userService)
        {
            this.tokenService = tokenService;
            this.userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] SignUp model)
        {
            var result = await userService.SignUp(model.Username ?? "a", model.Password ?? "b");
            if (result)
            {
                return Ok("User registered successfully.");
            }
            return BadRequest("User already exists.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SignIn model)
        {
            string? token = tokenService.GenerateToken(model);

            if (token == null || token.IsNullOrEmpty()) return Unauthorized();


            bool result = await userService.SignIn(model.Username ?? "a", model.Password ?? "b");
            if (result == true)
            {
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid credentials.");
        }

        [HttpGet("secure-data")]
        [Authorize]
        public IActionResult GetSecureData()
        {
            return Ok(new { Message = "This is protected data." });
        }

    }
}
