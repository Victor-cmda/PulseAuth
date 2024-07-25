using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [SwaggerOperation(OperationId = "RegisterUser")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerOperation("LoginUser")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("oauth/v2/token")]
        [SwaggerOperation("GetOauthToken")]
        public async Task<ActionResult<AuthResponseDto>> GenerateToken()
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
                return BadRequest(ex.Message);
            }
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
