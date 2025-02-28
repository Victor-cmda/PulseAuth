using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserWithClientByIdAsync(string userId);
        Task<Client> GetClientByUserIdAsync(string userId);
        Task UpdateClientAsync(Client client);
        Task SaveChangesAsync();
    }
}
