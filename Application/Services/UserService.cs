using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<UserConfigDto> GetConfigByUserIdAsync(Guid id)
        {
            try
            {
                var user = await _userRepository.GetUserConfigByUserIdAsync(id);
                if (user == null)
                {
                    return null;
                }

                var userConfigDto = new UserConfigDto
                {
                    ClientId = user.Client?.ClientId.ToString(),
                    ClientSecret = user.Client?.ClientSecret,
                    Callbacks = user.Callback != null ? new CallbackDto
                    {
                        Credit = user.Callback.Credit,
                        Debit = user.Callback.Debit,
                        Registration = user.Callback.Registration
                    } : new CallbackDto(),
                    Sellers = user.Sellers != null ? user.Sellers.Select(s => new SellerConfigDto
                    {
                        Name = s.Name,
                        Description = s.Description,
                        SellerId = s.Id.ToString()
                    }).ToList() : new List<SellerConfigDto>()
                };

                return userConfigDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the user configuration: {ex.Message}", ex);
            }
        }
    }
}