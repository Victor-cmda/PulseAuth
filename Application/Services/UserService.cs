using Application.DTOs;
using Application.Interfaces;
using AutoMapper.Internal;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class UserService : IUserService
    {
        public IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ConfigurationResponseDto> GetConfigurationUser(Guid userId)
        {
            var user = await _userRepository.GetUserWithClientByIdAsync(userId.ToString());

            if (user == null || user.Client == null)
            {
                throw new Exception("Usuário ou configurações de cliente não encontradas");
            }

            var configResponse = new ConfigurationResponseDto
            {
                ApiConfig = new ApiConfigDto
                {
                    ClientId = user.Client.ClientId,
                    ClientSecret = user.Client.ClientSecret,
                    ApiEndpoint = user.Client.ApiEndpoint ?? "https://api.pulsepay.com.br/v1"
                }
            };

            return configResponse;
        }

        public async Task<ConfigurationResponseDto> UpdateConfigurationUser(Guid userId, UpdateConfigurationDto updateDto)
        {
            var client = await _userRepository.GetClientByUserIdAsync(userId.ToString());

            if (client == null)
            {
                throw new Exception("Configurações de cliente não encontradas");
            }

            if (!string.IsNullOrEmpty(updateDto.ApiConfig?.ApiEndpoint))
            {
                client.ApiEndpoint = updateDto.ApiConfig.ApiEndpoint;
            }

            await _userRepository.UpdateClientAsync(client);
            await _userRepository.SaveChangesAsync();

            return new ConfigurationResponseDto
            {
                ApiConfig = new ApiConfigDto
                {
                    ClientId = client.ClientId,
                    ClientSecret = client.ClientSecret,
                    ApiEndpoint = client.ApiEndpoint
                }
            };
        }
    }
}
