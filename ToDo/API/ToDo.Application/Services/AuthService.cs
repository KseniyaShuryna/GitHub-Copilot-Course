using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ToDo.Infrastructure.Entities;
using ToDo.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ToDo.Application.Options;
using ToDo.Application.DTOs;
using ToDo.Application.Interfaces;
using ToDo.Application.Exceptions;

namespace ToDo.Application.Services
{
    /// <summary>
    /// Service for authentication, registration, and JWT token management.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ToDoDbContext _context;
        private readonly JwtOptions _jwtOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="jwtOptions">JWT configuration options.</param>
        public AuthService(ToDoDbContext context, IOptions<JwtOptions> jwtOptions)
        {
            _context = context;
            _jwtOptions = jwtOptions.Value;
        }

        /// <summary>
        /// Registers a new user and returns authentication tokens.
        /// </summary>
        /// <param name="request">The registration request data.</param>
        /// <returns>The authentication response with tokens.</returns>
        /// <exception cref="ValidationException">Thrown if the email already exists.</exception>
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new ValidationException("Email already exists");

            var user = new UserEntity
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = nameof(UserRole.User)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return await LoginAsync(new LoginRequestDto { Email = request.Email, Password = request.Password });
        }

        /// <summary>
        /// Authenticates a user and returns authentication tokens.
        /// </summary>
        /// <param name="request">The login request data.</param>
        /// <returns>The authentication response with tokens.</returns>
        /// <exception cref="UnauthorizedException">Thrown if credentials are invalid.</exception>
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid credentials");

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateSecureToken();
            var refreshTokenEntity = new RefreshTokenEntity
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
                IsRevoked = false
            };
            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Email = user.Email,
                Role = user.Role
            };
        }

        /// <summary>
        /// Refreshes the JWT access token using a refresh token.
        /// </summary>
        /// <param name="request">The refresh token request data.</param>
        /// <returns>The authentication response with new tokens.</returns>
        /// <exception cref="NotFoundException">Thrown if the refresh token or user is not found.</exception>
        /// <exception cref="UnauthorizedException">Thrown if the refresh token is invalid or expired.</exception>
        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var oldToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (oldToken == null)
                throw new NotFoundException("Refresh token not found");
            if (oldToken.IsRevoked || oldToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedException("Invalid or expired refresh token");

            // Revoke old token
            oldToken.IsRevoked = true;

            // Issue new refresh token
            var newRefreshToken = GenerateSecureToken();
            var newRefreshTokenEntity = new RefreshTokenEntity
            {
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = oldToken.UserId,
                IsRevoked = false
            };
            _context.RefreshTokens.Add(newRefreshTokenEntity);

            await _context.SaveChangesAsync();

            if (oldToken.User == null)
                throw new NotFoundException("Associated user not found for the refresh token.");

            var accessToken = GenerateJwtToken(oldToken.User);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                Email = oldToken.User.Email,
                Role = oldToken.User.Role
            };
        }

        /// <summary>
        /// Invalidates (revokes) the given refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke.</param>
        /// <exception cref="NotFoundException">If the token does not exist.</exception>
        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (token == null)
                throw new NotFoundException("Refresh token not found");
            if (!token.IsRevoked)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Generates a JWT access token for the specified user.
        /// </summary>
        /// <param name="user">The user entity.</param>
        /// <returns>The generated JWT token string.</returns>
        /// <exception cref="InvalidOperationException">Thrown if JWT configuration is missing.</exception>
        private string GenerateJwtToken(UserEntity user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            if (string.IsNullOrWhiteSpace(_jwtOptions.Key))
                throw new InvalidOperationException("JWT signing key is not configured.");
            if (string.IsNullOrWhiteSpace(_jwtOptions.Issuer) || string.IsNullOrWhiteSpace(_jwtOptions.Audience))
                throw new InvalidOperationException("JWT issuer or audience is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generates a secure random token for refresh tokens.
        /// </summary>
        /// <param name="size">The size of the token in bytes.</param>
        /// <returns>The generated secure token as a Base64 string.</returns>
        private string GenerateSecureToken(int size = 32)
        {
            var bytes = new byte[size];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }
    }
}
