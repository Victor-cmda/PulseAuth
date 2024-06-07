using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISellerService
    {
        Task<IEnumerable<Seller>> GetSellersByUserIdAsync(Guid id);
        Task<Seller> PostSellerAsync(SellerDto sellerDto);
        Task<Seller> PutSellerAsync(Guid Id, SellerDto sellerDto);
        Task<bool> DeleteSellerAsync(Guid Id);
    }
}
