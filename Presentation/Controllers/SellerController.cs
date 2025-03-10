using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("seller")]
    public class SellerController : ControllerBase
    {
        private readonly ISellerService _sellerService;
        private readonly ICommerceService _commerceService;
        private readonly ILogger<SellerController> _logger;


        public SellerController(ISellerService sellerService, ILogger<SellerController> logger, ICommerceService commerceService)
        {
            _sellerService = sellerService;
            _logger = logger;
            _commerceService = commerceService;
        }

        [HttpGet]
        [Route("available")]
        public async Task<ActionResult<IEnumerable<SellerDto>>> GetSellersByUserId()
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
        public async Task<ActionResult<SellerDto>> PostSeller(SellerDto sellerDto)
        {
            var userIdString = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Route available only for user type token");
            }
            var result = await _sellerService.PostSellerAsync(sellerDto, userId);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<SellerDto>> GetSellerById(Guid id)
        {
            try
            {
                var userIdString = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usu�rio inv�lido ou ausente");
                }

                var sellers = await _sellerService.GetSellersByUserIdAsync(userId);
                var seller = sellers.FirstOrDefault(s => s.Id == id);

                if (seller == null)
                {
                    return NotFound("Seller n�o encontrado ou n�o pertence ao usu�rio autenticado");
                }

                return Ok(seller);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter seller {SellerId}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicita��o");
            }
        }

        [HttpGet]
        [Route("{id}/with-commerces")]
        public async Task<ActionResult<SellerWithCommercesDto>> GetSellerWithCommerces(Guid id)
        {
            try
            {
                var userIdString = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usu�rio inv�lido ou ausente");
                }

                var sellers = await _sellerService.GetSellersByUserIdAsync(userId);
                var seller = sellers.FirstOrDefault(s => s.Id == id);

                if (seller == null)
                {
                    return NotFound("Seller n�o encontrado ou n�o pertence ao usu�rio autenticado");
                }

                var commerces = await _commerceService.GetCommercesBySellerIdAsync(id);

                var result = new SellerWithCommercesDto
                {
                    Id = seller.Id,
                    Name = seller.Name,
                    Description = seller.Description,
                    Commerces = commerces.ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter seller com com�rcios {SellerId}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicita��o");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SellerDto>> PutSeller(Guid id, SellerDto sellerDto)
        {
            try
            {
                var userIdString = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usu�rio inv�lido ou ausente");
                }

                var sellers = await _sellerService.GetSellersByUserIdAsync(userId);
                if (!sellers.Any(s => s.Id == id))
                {
                    return NotFound("Seller n�o encontrado ou n�o pertence ao usu�rio autenticado");
                }

                var result = await _sellerService.PutSellerAsync(id, sellerDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar seller {SellerId}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicita��o");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSeller(Guid id)
        {
            try
            {
                var userIdString = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usu�rio inv�lido ou ausente");
                }

                var sellers = await _sellerService.GetSellersByUserIdAsync(userId);
                if (!sellers.Any(s => s.Id == id))
                {
                    return NotFound("Seller n�o encontrado ou n�o pertence ao usu�rio autenticado");
                }

                var result = await _sellerService.DeleteSellerAsync(id);
                if (result)
                {
                    return NoContent();
                }
                else
                {
                    return StatusCode(500, "N�o foi poss�vel excluir o seller");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir seller {SellerId}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicita��o");
            }
        }

        private string GetUserIdFromToken()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            var userIdClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "sub");
            return userIdClaim?.Value;
        }
    }
}
