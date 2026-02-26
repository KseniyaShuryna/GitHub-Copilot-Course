using Microsoft.AspNetCore.Mvc;
using ToDo.Application.Interfaces;
using ToDo.Application.DTOs;
using ToDo.Api.Models;

namespace ToDo.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The registration request data.</param>
        /// <returns>The authentication response with tokens.</returns>
        /// <response code="200">Returns the authentication response.</response>
        /// <response code="400">If the request is invalid.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Authenticates a user and returns JWT tokens.
        /// </summary>
        /// <param name="request">The login request data.</param>
        /// <returns>The authentication response with tokens.</returns>
        /// <response code="200">Returns the authentication response.</response>
        /// <response code="401">If credentials are invalid.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Refreshes the JWT access token using a refresh token.
        /// </summary>
        /// <param name="request">The refresh token request data.</param>
        /// <returns>The authentication response with new tokens.</returns>
        /// <response code="200">Returns the authentication response.</response>
        /// <response code="401">If the refresh token is invalid or expired.</response>
        /// <response code="404">If the refresh token is not found.</response>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Logs out the user by invalidating the refresh token.
        /// </summary>
        /// <param name="request">The refresh token to invalidate.</param>
        /// <response code="204">Logout successful.</response>
        /// <response code="404">If the refresh token is not found.</response>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            await _authService.LogoutAsync(request.RefreshToken);
            return NoContent();
        }
    }
}
