using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Client> FindByClientIdAsync(Guid clientId)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == clientId.ToString());
        }
    }
}
