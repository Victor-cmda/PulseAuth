using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ISellerRepository
    {
        Task<IEnumerable<Seller>> GetSellersByUserIdAsync(Guid Id);
        Task<Seller> PostSellerAsync(Seller seller);
        Task<Seller> PutSellerAsync(Guid Id, Seller seller);
        Task<bool> DeleteSellerAsync(Guid Id);
    }
}
