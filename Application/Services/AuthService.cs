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
using System.Text.RegularExpressions;

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
            var baseUsername = GenerateBaseUsername(registerDto.Email);
            var username = await GenerateUniqueUsername(baseUsername);
            var clientId = Guid.NewGuid().ToString();
            var clientSecret = Guid.NewGuid().ToString();

            var user = new User
            {
                UserName = username,
                Email = registerDto.Email,
                Name = registerDto.Username,
                PhoneNumber = registerDto.PhoneNumber,
                Document = registerDto.Document,
                DocumentType = registerDto.DocumentType,
                IsForeigner = registerDto.IsForeigner,
                Nationality = registerDto.IsForeigner == true ? registerDto.Nationality : null,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                Client = new Client
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    ApiEndpoint = _configuration.GetSection("Client")["ApiEndpoint"] ?? "",
                }
            };

            if (user.IsForeigner && string.IsNullOrEmpty(user.Nationality))
            {
                throw new ArgumentException("Nationality is required for foreign users");
            }

            if (!user.IsForeigner && (user.DocumentType != "CPF" && user.DocumentType != "CNPJ"))
            {
                throw new ArgumentException("DocumentType must be either CPF or CNPJ for Brazilian users");
            }

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to register user");
            }

            return GenerateTokenForUser(user);
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

            return GenerateTokenForUser(user);
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

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Client.Id == clientId);
            return user;
        }

        private AuthResponseDto GenerateTokenForUser(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");

            var roles = _userManager.GetRolesAsync(user).Result;

            var claims = new List<Claim>
            {
                new Claim("sub", user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim("TokenType", "User")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
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
                ExpiresIn = token.ValidTo,
                User = new UserDto
                {
                    Name = user.Name,
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                }
            };
        }

        private string GenerateBaseUsername(string email)
        {
            var baseUsername = email.Split('@')[0];

            baseUsername = Regex.Replace(baseUsername, @"[^a-zA-Z0-9]", "");

            if (baseUsername.Length > 20)
            {
                baseUsername = baseUsername.Substring(0, 20);
            }

            return baseUsername.ToLower();
        }

        private async Task<string> GenerateUniqueUsername(string baseUsername)
        {
            var username = baseUsername;
            var counter = 1;

            while (await _userManager.FindByNameAsync(username) != null)
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            return username;
        }
    }
}
