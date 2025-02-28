using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class CommerceService : ICommerceService
    {
        private readonly ICommerceRepository _commerceRepository;
        private readonly ICommerceCallbackRepository _callbackRepository;

        public CommerceService(
            ICommerceRepository commerceRepository,
            ICommerceCallbackRepository callbackRepository)
        {
            _commerceRepository = commerceRepository;
            _callbackRepository = callbackRepository;
        }

        public async Task<IEnumerable<CommerceDto>> GetCommercesBySellerIdAsync(Guid sellerId)
        {
            try
            {
                var commerces = await _commerceRepository.GetCommercesBySellerId(sellerId);

                return commerces.Select(c => new CommerceDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Url = c.Url,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt.ToString("dd/MM/yyyy"),
                    Callbacks = c.Callback != null ? new CommerceCallbackDto
                    {
                        Credit = c.Callback.Credit,
                        Debit = c.Callback.Debit,
                        Boleto = c.Callback.Boleto,
                        Webhook = c.Callback.Webhook,
                        SecurityKey = c.Callback.SecurityKey
                    } : null
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erro ao buscar comércios do seller: {ex.Message}", ex);
            }
        }

        public async Task<CommerceDto> GetCommerceByIdAsync(Guid id)
        {
            try
            {
                var commerce = await _commerceRepository.GetCommerceByIdAsync(id);

                if (commerce == null)
                    return null;

                return new CommerceDto
                {
                    Id = commerce.Id,
                    Name = commerce.Name,
                    Url = commerce.Url,
                    Status = commerce.Status,
                    CreatedAt = commerce.CreatedAt.ToString("dd/MM/yyyy"),
                    Callbacks = commerce.Callback != null ? new CommerceCallbackDto
                    {
                        Credit = commerce.Callback.Credit,
                        Debit = commerce.Callback.Debit,
                        Boleto = commerce.Callback.Boleto,
                        Webhook = commerce.Callback.Webhook,
                        SecurityKey = commerce.Callback.SecurityKey
                    } : null
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erro ao buscar comércio: {ex.Message}", ex);
            }
        }

        public async Task<CommerceDto> CreateCommerceAsync(CommerceCreateDto dto, Guid sellerId)
        {
            try
            {
                var commerce = new Commerce
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Url = dto.Url,
                    Status = "active",
                    CreatedAt = DateTime.UtcNow,
                    SellerId = sellerId
                };

                var result = await _commerceRepository.CreateCommerceAsync(commerce);

                return new CommerceDto
                {
                    Id = result.Id,
                    Name = result.Name,
                    Url = result.Url,
                    Status = result.Status,
                    CreatedAt = result.CreatedAt.ToString("dd/MM/yyyy")
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erro ao criar comércio: {ex.Message}", ex);
            }
        }

        public async Task<CommerceDto> UpdateCommerceAsync(Guid id, CommerceUpdateDto dto)
        {
            try
            {
                var commerce = new Commerce
                {
                    Name = dto.Name,
                    Url = dto.Url,
                    Status = "active"
                };

                var result = await _commerceRepository.UpdateCommerceAsync(id, commerce);

                if (result == null)
                    return null;

                var callback = await _callbackRepository.GetCallbackByCommerceIdAsync(id);

                return new CommerceDto
                {
                    Id = result.Id,
                    Name = result.Name,
                    Url = result.Url,
                    Status = result.Status,
                    CreatedAt = result.CreatedAt.ToString("dd/MM/yyyy"),
                    Callbacks = callback != null ? new CommerceCallbackDto
                    {
                        Credit = callback.Credit,
                        Debit = callback.Debit,
                        Boleto = callback.Boleto,
                        Webhook = callback.Webhook,
                        SecurityKey = callback.SecurityKey
                    } : null
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erro ao atualizar comércio: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteCommerceAsync(Guid id)
        {
            try
            {
                return await _commerceRepository.DeleteCommerceAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erro ao excluir comércio: {ex.Message}", ex);
            }
        }

        public async Task<CommerceCallbackDto> UpdateCommerceCallbackAsync(Guid commerceId, CommerceCallbackUpdateDto dto)
        {
            try
            {
                var commerce = await _commerceRepository.GetCommerceByIdAsync(commerceId);
                if (commerce == null)
                    throw new ApplicationException("Comércio não encontrado");

                var callback = new CommerceCallback
                {
                    Credit = dto.Credit,
                    Debit = dto.Debit,
                    Boleto = dto.Boleto,
                    Webhook = dto.Webhook,
                    SecurityKey = dto.SecurityKey
                };

                var result = await _callbackRepository.UpdateCallbackAsync(commerceId, callback);

                return new CommerceCallbackDto
                {
                    Credit = result.Credit,
                    Debit = result.Debit,
                    Boleto = result.Boleto,
                    Webhook = result.Webhook,
                    SecurityKey = result.SecurityKey
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erro ao atualizar callbacks: {ex.Message}", ex);
            }
        }
    }

}
