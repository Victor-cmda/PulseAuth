using Application.DTOs;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<ConfigurationResponseDto> GetConfigurationUser(Guid userId);
        Task<ConfigurationResponseDto> UpdateConfigurationUser(Guid userId, UpdateConfigurationDto updateDto);
    }
}

