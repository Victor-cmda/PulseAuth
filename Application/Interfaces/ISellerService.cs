using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISellerService
    {
        Task<IEnumerable<SellerDto>> GetSellersByUserIdAsync(Guid id);
        Task<SellerDto> PostSellerAsync(SellerDto sellerDto, Guid userId);
        Task<SellerDto> PutSellerAsync(Guid Id, SellerDto sellerDto);
        Task<bool> DeleteSellerAsync(Guid Id);
    }
}
