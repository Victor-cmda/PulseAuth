using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRoleService _roleService;
        public AuthController(IAuthService authService, IRoleService roleService)
        {
            _authService = authService;
            _roleService = roleService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("oauth/v2/token")]
        public async Task<IActionResult> GenerateToken()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Unauthorized("Missing Authorization Header");
            }

            try
            {
                var authorizationHeader = Request.Headers["Authorization"].ToString();
                var clientCredentialsDto = DecodeBasicAuthenticationHeader(authorizationHeader);

                var user = await _authService.ValidateClientCredentialsAsync(clientCredentialsDto.ClientId, clientCredentialsDto.ClientSecret);
                if (user == null)
                {
                    return Unauthorized("Invalid credentials");
                }

                var token = await _authService.GenerateTokenAsync(clientCredentialsDto);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("assign-admin")]
        [Authorize(Policy = "AdminPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AssignAdminRole([FromBody] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("UserId is required");
            }

            var result = await _roleService.AssignUserToRoleAsync(userId, "Admin");
            if (!result)
            {
                return BadRequest("Failed to assign admin role to user");
            }

            return Ok(new { message = "Admin role assigned successfully" });
        }

        [HttpGet("check-admin")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult CheckAdmin()
        {
            return Ok(new { isAdmin = true });
        }

        private ClientCredentialsDto DecodeBasicAuthenticationHeader(string authorizationHeader)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Basic "))
            {
                throw new ArgumentException("Invalid Authorization header.");
            }

            var encodedCredentials = authorizationHeader.Substring("Basic ".Length).Trim();
            var decodedBytes = Convert.FromBase64String(encodedCredentials);
            var decodedCredentials = Encoding.UTF8.GetString(decodedBytes);

            var credentialsParts = decodedCredentials.Split(':');
            if (credentialsParts.Length != 2)
            {
                throw new ArgumentException("Invalid credentials format.");
            }

            return new ClientCredentialsDto
            {
                ClientId = Guid.Parse(credentialsParts[0]),
                ClientSecret = credentialsParts[1]
            };
        }
    }
}
