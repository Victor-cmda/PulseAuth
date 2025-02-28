using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class CommerceCallbackRepository : ICommerceCallbackRepository
    {
        private readonly AppDbContext _context;

        public CommerceCallbackRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CommerceCallback> GetCallbackByCommerceIdAsync(Guid commerceId)
        {
            return await _context.CommerceCallbacks
                .FirstOrDefaultAsync(c => c.CommerceId == commerceId);
        }

        public async Task<CommerceCallback> CreateCallbackAsync(CommerceCallback callback)
        {
            _context.CommerceCallbacks.Add(callback);
            await _context.SaveChangesAsync();
            return callback;
        }

        public async Task<CommerceCallback> UpdateCallbackAsync(Guid commerceId, CommerceCallback callback)
        {
            var existingCallback = await _context.CommerceCallbacks
                .FirstOrDefaultAsync(c => c.CommerceId == commerceId);

            if (existingCallback == null)
            {
                callback.CommerceId = commerceId;
                return await CreateCallbackAsync(callback);
            }

            existingCallback.Credit = callback.Credit;
            existingCallback.Debit = callback.Debit;
            existingCallback.Boleto = callback.Boleto;
            existingCallback.Webhook = callback.Webhook;
            existingCallback.SecurityKey = callback.SecurityKey;

            await _context.SaveChangesAsync();
            return existingCallback;
        }
    }
}
