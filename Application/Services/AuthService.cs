using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Infrastructure.Data;

namespace Application.Services
{
    public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, AppDbContext context)
    {
        _userManager = userManager;
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

        var clientId = Guid.NewGuid().ToString();
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
        // Aqui você pode implementar a lógica para gerar tokens com clientId e clientSecret
        throw new NotImplementedException();
    }

    private async Task<AuthResponseDto> GenerateTokenForUser(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
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
