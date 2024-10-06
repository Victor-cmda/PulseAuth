using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityUser> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<User> GetUserConfigByUserIdAsync(Guid Id);
        Task<Callback> GetCallbackBySellerIdAsync(Guid Id);
    }
}
