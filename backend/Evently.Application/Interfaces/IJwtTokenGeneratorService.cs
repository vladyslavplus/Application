using Evently.Application.DTOs.Auth;

namespace Evently.Application.Interfaces
{
    public interface IJwtTokenGeneratorService
    {
        Task<(string token, DateTime expiresAt)> GenerateTokenAsync(JwtUserDto user, IEnumerable<string> roles);
    }
}
