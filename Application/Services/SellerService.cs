using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class SellerService : ISellerService
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly IConfiguration _configuration;

        public SellerService(ISellerRepository sellerRepository, IConfiguration configuration)
        {
            _sellerRepository = sellerRepository;
            _configuration = configuration;
        }

        public async Task<bool> DeleteSellerAsync(Guid id)
        {
            try
            {
                return await _sellerRepository.DeleteSellerAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting the seller: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SellerDto>> GetSellersByUserIdAsync(Guid Id)
        {
            try
            {
                var sellers = await _sellerRepository.GetSellersByUserIdAsync(Id);

                var sellerDtos = new List<SellerDto>();
                foreach (var seller in sellers)
                {
                    sellerDtos.Add(new SellerDto
                    {
                        Id = seller.Id,
                        Name = seller.Name,
                        Description = seller.Description
                    });
                }

                return sellerDtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the sellers: {ex.Message}", ex);
            }
        }

        public async Task<SellerDto> PostSellerAsync(SellerDto sellerDto, Guid userId)
        {
            try
            {
                var seller = new Seller
                {
                    Id = Guid.NewGuid(),
                    Name = sellerDto.Name,
                    Description = sellerDto.Description,
                    UserId = userId.ToString()
                };

                var result = await _sellerRepository.PostSellerAsync(seller);

                return new SellerDto
                {
                    Id = result.Id,
                    Name = result.Name,
                    Description = result.Description,
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while adding the seller: {ex.Message}", ex);
            }
        }

        public async Task<SellerDto> PutSellerAsync(Guid id, SellerDto sellerDto)
        {
            try
            {
                var seller = new Seller
                {
                    Name = sellerDto.Name,
                    Description = sellerDto.Description,
                };

                var result = await _sellerRepository.PutSellerAsync(id, seller);

                return new SellerDto
                {
                    Id = result.Id,
                    Name = result.Name,
                    Description = result.Description,
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating the seller: {ex.Message}", ex);
            }
        }
    }
}