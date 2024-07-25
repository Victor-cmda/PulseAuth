using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Swashbuckle.AspNetCore.Annotations;

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

        #region Private Methods
        private string GetUserIdFromToken()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "sub");
            return userIdClaim?.Value;
        }
        #endregion

        [HttpGet]
        [Authorize]
        [Route("available")]
        [SwaggerOperation(OperationId = "GetAvailableSellers")]
        public async Task<ActionResult<IEnumerable<SellerResponseDto>>> GetSellersByUserId()
        {
            var userIdString = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Route available only for user type token");
            }
            var result = await _sellerService.GetSellersByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(OperationId = "CreateSeller")]
        public async Task<ActionResult<SellerResponseDto>> PostSeller(SellerDto sellerDto)
        {
            var userIdString = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Route available only for user type token");
            }
            var result = await _sellerService.PostSellerAsync(sellerDto, userId);
            return Ok(result);
        }
    }
}
