using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICommerceCallbackRepository
    {
        Task<CommerceCallback> GetCallbackByCommerceIdAsync(Guid commerceId);
        Task<CommerceCallback> CreateCallbackAsync(CommerceCallback callback);
        Task<CommerceCallback> UpdateCallbackAsync(Guid commerceId, CommerceCallback callback);
    }
}
