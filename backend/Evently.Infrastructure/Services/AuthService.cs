using Evently.Application.DTOs.Auth;
using Evently.Application.Interfaces;
using Evently.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Evently.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGeneratorService _jwtTokenGenerator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenGeneratorService jwtTokenGenerator,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Attempt to register with existing email: {Email}", request.Email);
                return null;
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FullName = request.FullName,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("User registration failed: {Errors}", errors);
                return null;
            }

            await _userManager.AddToRoleAsync(user, "User");

            var roles = await _userManager.GetRolesAsync(user);

            var jwtUser = new JwtUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName
            };

            var (token, expiresAt) = await _jwtTokenGenerator.GenerateTokenAsync(jwtUser, roles);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: user with email {Email} not found", request.Email);
                return null;
            }

            var passwordValid = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!passwordValid.Succeeded)
            {
                _logger.LogWarning("Invalid password for user {Email}", request.Email);
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var jwtUser = new JwtUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName
            };

            var (token, expiresAt) = await _jwtTokenGenerator.GenerateTokenAsync(jwtUser, roles);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt
            };
        }
    }
}
