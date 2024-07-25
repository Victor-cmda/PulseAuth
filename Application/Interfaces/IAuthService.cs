using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> GenerateTokenAsync(ClientCredentialsDto clientCredentialsDto);
        Task<User> ValidateClientCredentialsAsync(Guid clientId, string clientSecret);
    }
}
