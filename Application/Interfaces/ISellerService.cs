using Application.DTOs;

namespace Application.Interfaces
{
    public interface ISellerService
    {
        Task<IEnumerable<SellerResponseDto>> GetSellersByUserIdAsync(Guid id);
        Task<SellerResponseDto> PostSellerAsync(SellerDto sellerDto, Guid userId);
        Task<SellerResponseDto> PutSellerAsync(Guid Id, SellerDto sellerDto);
        Task<bool> DeleteSellerAsync(Guid Id);
    }
}
