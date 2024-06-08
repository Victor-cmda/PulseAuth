using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Seller>> GetSellersByUserIdAsync(Guid id)
        {
            try
            {
                return await _sellerRepository.GetSellersByUserIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the sellers: {ex.Message}", ex);
            }
        }

        public async Task<Seller> PostSellerAsync(SellerDto sellerDto)
        {
            try
            {
                var seller = new Seller
                {
                    SellerId = Guid.NewGuid(),
                    Name = sellerDto.Name,
                    Description = sellerDto.Description,
                };

                var result = await _sellerRepository.PostSellerAsync(seller);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while adding the seller: {ex.Message}", ex);
            }
        }

        public async Task<Seller> PutSellerAsync(Guid id, SellerDto sellerDto)
        {
            try
            {
                var seller = new Seller
                {
                    Name = sellerDto.Name,
                    Description = sellerDto.Description,
                };

                var result = await _sellerRepository.PutSellerAsync(id, seller);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating the seller: {ex.Message}", ex);
            }
        }
    }
}