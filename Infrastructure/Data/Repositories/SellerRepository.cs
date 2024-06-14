using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class SellerRepository : ISellerRepository
    {
        private readonly AppDbContext _context;

        public SellerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Seller>> GetSellersByUserIdAsync(Guid id)
        {
            return await _context.Sellers.Where(x => x.UserId == id).ToListAsync();
        }

        public async Task<Seller> PostSellerAsync(Seller seller)
        {
            await _context.Sellers.AddAsync(seller);
            await _context.SaveChangesAsync();
            return seller;
        }

        public async Task<Seller> PutSellerAsync(Guid id, Seller seller)
        {
            if (id != seller.SellerId)
            {
                throw new ArgumentException("SellerId Not Found");
            }

            var existingSeller = await _context.Sellers.FirstOrDefaultAsync(m => m.SellerId == id);

            if (existingSeller != null)
            {
                existingSeller.Name = seller.Name;
                existingSeller.Description = seller.Description;
                await _context.SaveChangesAsync();
                return existingSeller;
            }
            else
            {
                throw new ArgumentException("Seller Not Found");
            }
        }

        public async Task<bool> DeleteSellerAsync(Guid id)
        {
            var seller = await _context.Sellers.FirstOrDefaultAsync(m => m.SellerId == id);
            if (seller != null)
            {
                _context.Sellers.Remove(seller);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new ArgumentException("Seller Not Found");
            }
        }
    }
}