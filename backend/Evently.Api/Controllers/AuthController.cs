using Evently.Application.DTOs.Auth;
using Evently.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evently.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="request">User registration data.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>JWT token and expiration date.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);
            if (result == null)
                return BadRequest(new { Message = "Registration failed. Email may already be in use." });

            return Ok(result);
        }

        /// <summary>
        /// Logs in an existing user with email and password.
        /// </summary>
        /// <param name="request">User login credentials.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>JWT token and expiration date.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);
            if (result == null)
                return Unauthorized(new { Message = "Invalid email or password." });

            return Ok(result);
        }
    }
}
