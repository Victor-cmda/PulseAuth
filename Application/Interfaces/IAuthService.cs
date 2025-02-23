using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> GenerateTokenAsync(ClientCredentialsDto clientCredentialsDto);
        Task<User> ValidateClientCredentialsAsync(Guid clientId, string clientSecret);
    }
}
