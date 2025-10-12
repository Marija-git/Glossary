using Glossary.API.DTOs.Request;
using Glossary.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Glossary.API.Controllers
{
    [Route("api/identity")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDtoRequest dto)
        {
            await _authService.Register(dto.Username, dto.Password,dto.Email);
            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDtoRequest dto)
        {
            var token = await _authService.Login(dto.Username, dto.Password);
            return Ok(new { token });
        }
    }
}
