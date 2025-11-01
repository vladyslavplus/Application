namespace Evently.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
