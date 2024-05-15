using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityUser> GetByEmailAsync(string email);
        Task AddAsync(IdentityUser user);
    }
}
