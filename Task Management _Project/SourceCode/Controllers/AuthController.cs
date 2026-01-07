using Microsoft.AspNetCore.Mvc;
using InternalProcessMgmt.Services;
using InternalProcessMgmt.Models.DTOs;

namespace InternalProcessMgmt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var user = await _auth.RegisterAsync(dto.Username, dto.Email, dto.Password, dto.Role);

            if (user == null)
                return BadRequest(new { message = "Username already exists" });

            return Ok(new
            {
                message = "User created successfully",
                user = new { user.UserId, user.Username, user.Email, user.RoleId }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var token = await _auth.AuthenticateAsync(dto.Username, dto.Password);

            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new { token });
        }
    }
}
