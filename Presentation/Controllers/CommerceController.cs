using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("commerce")]
    [Authorize]
    public class CommerceController : ControllerBase
    {
        private readonly ICommerceService _commerceService;
        private readonly ISellerService _sellerService;
        private readonly ILogger<CommerceController> _logger;

        public CommerceController(
            ICommerceService commerceService,
            ISellerService sellerService,
            ILogger<CommerceController> logger)
        {
            _commerceService = commerceService;
            _sellerService = sellerService;
            _logger = logger;
        }

        [HttpGet]
        [Route("seller/{sellerId}")]
        public async Task<ActionResult<IEnumerable<CommerceDto>>> GetCommercesBySellerId(Guid sellerId)
        {
            try
            {
                var userIdString = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usuário inválido ou ausente");
                }

                var sellers = await _sellerService.GetSellersByUserIdAsync(userId);
                if (!sellers.Any(s => s.Id == sellerId))
                {
                    return Forbid("O Seller não pertence ao usuário autenticado");
                }

                var result = await _commerceService.GetCommercesBySellerIdAsync(sellerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter comércios para o seller {SellerId}", sellerId);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CommerceDto>> GetCommerceById(Guid id)
        {
            try
            {
                var userIdString = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usuário inválido ou ausente");
                }

                var result = await _commerceService.GetCommerceByIdAsync(id);
                if (result == null)
                {
                    return NotFound("Comércio não encontrado");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter comércio {CommerceId}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        [HttpPost]
        [Route("{sellerId}")]
        public async Task<ActionResult<CommerceDto>> CreateCommerce(Guid sellerId, CommerceCreateDto dto)
        {
            try
            {
                var userIdString = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usuário inválido ou ausente");
                }

                var sellers = await _sellerService.GetSellersByUserIdAsync(userId);
                if (!sellers.Any(s => s.Id == sellerId))
                {
                    return Forbid("O Seller não pertence ao usuário autenticado");
                }

                var result = await _commerceService.CreateCommerceAsync(dto, sellerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar comércio para o seller {SellerId}", sellerId);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<CommerceDto>> UpdateCommerce(Guid id, CommerceUpdateDto dto)
        {
            try
            {
                var userIdString = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usuário inválido ou ausente");
                }

                var result = await _commerceService.UpdateCommerceAsync(id, dto);
                if (result == null)
                {
                    return NotFound("Comércio não encontrado");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar comércio {CommerceId}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteCommerce(Guid id)
        {
            try
            {
                var userIdString = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usuário inválido ou ausente");
                }

                var result = await _commerceService.DeleteCommerceAsync(id);
                if (!result)
                {
                    return NotFound("Comércio não encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir comércio {CommerceId}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        [HttpPut]
        [Route("{id}/callback")]
        public async Task<ActionResult<CommerceCallbackDto>> UpdateCallback(Guid id, CommerceCallbackUpdateDto dto)
        {
            try
            {
                var userIdString = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usuário inválido ou ausente");
                }

                var result = await _commerceService.UpdateCommerceCallbackAsync(id, dto);
                return Ok(result);
            }
            catch (ApplicationException ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar callbacks do comércio {CommerceId}", id);
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação");
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
