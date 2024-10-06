using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserConfigDto> GetConfigByUserIdAsync(Guid id);
        Task<CallbackDto> GetCallbackBySellerIdAsync(Guid id);
    }
}
