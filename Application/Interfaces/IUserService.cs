using Application.DTOs;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<ClientCredentialsDto> GetConfigurationUser(Guid userId);
    }
}

