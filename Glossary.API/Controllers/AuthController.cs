using Glossary.API.DTOs;
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
            var result = await _authService.Register(dto.Username, dto.Password,dto.Email);
            if (!result.Succeeded) return BadRequest(result.Errors);   //handle err
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDtoRequest dto)
        {
            var token = await _authService.Login(dto.Username, dto.Password);
            if (token == null) return Unauthorized();                                   //handle err
            return Ok(new { token });
        }
    }
}
