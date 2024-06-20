using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IClientRepository _clientRepository;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, AppDbContext context, IClientRepository clientRepository)
        {
            _userManager = userManager;
            _clientRepository = clientRepository;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to register user");
            }

            var clientId = Guid.NewGuid();
            var clientSecret = Guid.NewGuid().ToString();

            var client = new Client
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                UserId = user.Id,
                User = user
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return await GenerateTokenForUser(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                throw new Exception("Invalid credentials");
            }

            return await GenerateTokenForUser(user);
        }

        public async Task<AuthResponseDto> GenerateTokenAsync(ClientCredentialsDto clientCredentialsDto)
        {
            var client = await _clientRepository.FindByClientIdAsync(clientCredentialsDto.ClientId);
            if (client == null || client.ClientSecret != clientCredentialsDto.ClientSecret)
            {
                throw new UnauthorizedAccessException("Invalid client credentials");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, client.ClientId.ToString()),
                    new Claim(ClaimTypes.Hash, client.ClientSecret),
                    new Claim("TokenType", "Client")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration.GetSection("Jwt")["Issuer"],
                Audience = _configuration.GetSection("Jwt")["Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new AuthResponseDto
            {
                AccessToken = tokenString,
                ExpiresIn = token.ValidTo
            };
        }

        public async Task<User> ValidateClientCredentialsAsync(Guid clientId, string clientSecret)
        {
            var client = await _clientRepository.FindByClientIdAsync(clientId);
            if (client == null || client.ClientSecret != clientSecret)
            {
                return null;
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Client.ClientId == clientId);
            return user;
        }

        private async Task<AuthResponseDto> GenerateTokenForUser(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("sub", user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("TokenType", "User")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration.GetSection("Jwt")["Issuer"],
                Audience = _configuration.GetSection("Jwt")["Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new AuthResponseDto
            {
                AccessToken = tokenString,
                ExpiresIn = token.ValidTo
            };
        }
    }
}
