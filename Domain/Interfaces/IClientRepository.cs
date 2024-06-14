using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<Client> FindByClientIdAsync(string clientId);
    }
}
