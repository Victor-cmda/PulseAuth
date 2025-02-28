using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICommerceRepository
    {
        Task<IEnumerable<Commerce>> GetCommercesBySellerId(Guid sellerId);
        Task<Commerce> GetCommerceByIdAsync(Guid id);
        Task<Commerce> CreateCommerceAsync(Commerce commerce);
        Task<Commerce> UpdateCommerceAsync(Guid id, Commerce commerce);
        Task<bool> DeleteCommerceAsync(Guid id);
    }
}
