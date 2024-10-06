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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<User> GetUserConfigByUserIdAsync(Guid Id)
        {
            var userId = Id.ToString();

            return await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Callback)
                .Include(u => u.Client)
                .Include(u => u.Sellers)
                .SingleOrDefaultAsync();
        }

        public async Task<Callback> GetCallbackBySellerIdAsync(Guid Id)
        {
            return await _context.Callbacks.FirstOrDefaultAsync(x=>x.User.Sellers.Any(x=>x.Id == Id));
        }
    }
}
