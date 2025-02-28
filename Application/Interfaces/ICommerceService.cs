using Application.DTOs;

namespace Application.Interfaces
{
    public interface ICommerceService
    {
        Task<IEnumerable<CommerceDto>> GetCommercesBySellerIdAsync(Guid sellerId);
        Task<CommerceDto> GetCommerceByIdAsync(Guid id);
        Task<CommerceDto> CreateCommerceAsync(CommerceCreateDto dto, Guid sellerId);
        Task<CommerceDto> UpdateCommerceAsync(Guid id, CommerceUpdateDto dto);
        Task<bool> DeleteCommerceAsync(Guid id);
        Task<CommerceCallbackDto> UpdateCommerceCallbackAsync(Guid commerceId, CommerceCallbackUpdateDto dto);
    }
}
