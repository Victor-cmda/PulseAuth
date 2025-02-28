using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class CommerceRepository : ICommerceRepository
    {
        private readonly AppDbContext _context;

        public CommerceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Commerce>> GetCommercesBySellerId(Guid sellerId)
        {
            return await _context.Commerces
                .Include(c => c.Callback)
                .Where(c => c.SellerId == sellerId)
                .ToListAsync();
        }

        public async Task<Commerce> GetCommerceByIdAsync(Guid id)
        {
            return await _context.Commerces
                .Include(c => c.Callback)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Commerce> CreateCommerceAsync(Commerce commerce)
        {
            _context.Commerces.Add(commerce);
            await _context.SaveChangesAsync();
            return commerce;
        }

        public async Task<Commerce> UpdateCommerceAsync(Guid id, Commerce commerce)
        {
            var existingCommerce = await _context.Commerces.FindAsync(id);

            if (existingCommerce == null)
                return null;

            existingCommerce.Name = commerce.Name;
            existingCommerce.Url = commerce.Url;
            existingCommerce.Status = commerce.Status;

            await _context.SaveChangesAsync();
            return existingCommerce;
        }

        public async Task<bool> DeleteCommerceAsync(Guid id)
        {
            var commerce = await _context.Commerces.FindAsync(id);

            if (commerce == null)
                return false;

            _context.Commerces.Remove(commerce);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
