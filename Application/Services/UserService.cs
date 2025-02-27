using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IClientRepository _clientRepository;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, AppDbContext context, IClientRepository clientRepository)
        {
            _userManager = userManager;
            _clientRepository = clientRepository;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        public Task<ClientCredentialsDto> GetConfigurationUser(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
