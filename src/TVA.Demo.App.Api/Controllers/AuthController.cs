using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Application.Services;
using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;

namespace TVA.Demo.App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserService userService, ITokenService tokenService, ILogger<AuthController> logger) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<AuthController> _logger = logger;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (success, message, user) = await _userService.RegisterUserAsync(model.Username, model.Email, model.Password, cancellationToken);

            if (!success)
            {
                return BadRequest(new { Message = message });
            }
            _logger.LogInformation("User {Username} registered successfully.", model.Username);

            return Ok(new { Message = "User registered successfully. Please log in." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userService.AuthenticateUserAsync(model.UsernameOrEmail, model.Password, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Login failed for {UsernameOrEmail}.", model.UsernameOrEmail);
                return Unauthorized(new { Message = "Invalid username/email or password." });
            }

            var tokenString = _tokenService.GenerateJwtToken(user);
            _logger.LogInformation("User {Username} logged in successfully.", user.Username);
            return Ok(new AuthResponseDto
            {
                Token = tokenString,
                Expiration = _tokenService.GetTokenExpiry(),
                Username = user.Username
            });
        }
    }
}
