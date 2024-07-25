using Application.DTOs;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserConfigDto> GetConfigByUserIdAsync(Guid id);
    }
}
