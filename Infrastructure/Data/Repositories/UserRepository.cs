using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IdentityUser> GetByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserWithClientByIdAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Client)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<Client> GetClientByUserIdAsync(string userId)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task UpdateClientAsync(Client client)
        {
            _context.Clients.Update(client);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
