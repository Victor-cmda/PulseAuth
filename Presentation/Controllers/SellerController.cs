using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("seller")]
    public class SellerController : ControllerBase
    {
        private readonly ISellerService _sellerService;

        public SellerController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seller>>> GetSellersByUserId(Guid Id){
            var result = await _sellerService.GetSellersByUserIdAsync(Id);
            return Ok(result);
        }

        
    }
}
