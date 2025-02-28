using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("configuration")] 
        [ProducesResponseType(typeof(ConfigurationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetConfiguration()
        {
            try
            {
                var userIdString = GetUserIdFromToken();
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usuário inválido ou ausente");
                }

                var result = await _userService.GetConfigurationUser(userId);
                if (result == null)
                {
                    return NotFound("Configuração não encontrada para o usuário");
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter configuração para o usuário");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        [HttpPut("configuration")]
        [ProducesResponseType(typeof(ConfigurationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateConfiguration([FromBody] UpdateConfigurationDto updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    return BadRequest("Dados de atualização inválidos");
                }

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usuário inválido ou ausente");
                }

                var result = await _userService.UpdateConfigurationUser(userId, updateDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar configuração para o usuário");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        [HttpPost("configuration")]
        [ProducesResponseType(typeof(ConfigurationResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateConfiguration([FromBody] UpdateConfigurationDto createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return BadRequest("Dados de criação inválidos");
                }

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    return BadRequest("Token de usuário inválido ou ausente");
                }

                var result = await _userService.UpdateConfigurationUser(userId, createDto);

                return Created($"api/user/configuration", result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar configuração para o usuário");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao processar sua solicitação");
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
