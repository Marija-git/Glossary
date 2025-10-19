using Glossary.BusinessLogic.Exceptions;
using Glossary.BusinessLogic.Services.Interfaces;
using Glossary.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Glossary.BusinessLogic.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<User> userManager, IConfiguration config, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _config = config;
            _logger = logger;
        }

        public async Task Register(string username, string password,string email)
        {
            _logger.LogInformation("Register attempt for username: {Username}", username);

            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                _logger.LogWarning("Username '{Username}' is already taken", username);
                throw new BadRequestException($"Username '{username}' is already taken.");
            }
                
            var user = new User
            {
                UserName = username,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to register user {Username}: {Errors}", username, errors);
                throw new BadRequestException(errors);
            }
            _logger.LogInformation("User {Username} successfully registered", username);
        }

        public async Task<string> Login(string username, string password)
        {
            _logger.LogInformation("Login attempt for username: {Username}", username);

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("Login failed: user {Username} not found", username);
                throw new NotFoundException(nameof(User), username);
            }
                
            var validPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!validPassword)
            {
                _logger.LogWarning("Login failed: invalid password for user {Username}", username);
                throw new UnauthorizedException("Invalid username or password.");
            }

            var token = await GenerateToken(user);
            _logger.LogInformation("User {Username} successfully logged in", username);

            return token;
        }

        private async Task<string> GenerateToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
