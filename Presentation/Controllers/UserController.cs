using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ApiKeySettings _apiKeySettings;

        public UserController(IUserService userService, IOptions<ApiKeySettings> apiKeySettings)
        {
            _userService = userService;
            _apiKeySettings = apiKeySettings.Value;
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
        [Route("config")]
        [Authorize]
        public async Task<ActionResult<UserConfigDto>> GetConfigByUser()
        {
            var userIdString = GetUserIdFromToken();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return BadRequest("Route available only for user type token");
            }
            var result = await _userService.GetConfigByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpGet]
        [Route("callback/{sellerId}")]
        public async Task<ActionResult<CallbackDto>> GetConfigByUser([FromHeader(Name = "X-API-KEY")] string apiKey, Guid sellerId)
        {
            if (apiKey != _apiKeySettings.TransactionApiKey)
            {
                return Unauthorized("Invalid API key.");
            }

            var result = await _userService.GetCallbackBySellerIdAsync(sellerId);
            return Ok(result);
        }
    }
}
