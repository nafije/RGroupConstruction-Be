using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Common.Constants;
using RGroupConstruction.Application.Common.Error;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace RGroupConstruction.Application.Services;

public class AuthenticationService(
    IErrorService _errorService,
    IPasswordService _passwordService,
    ILogger<AuthenticationService> _logger,
    IUnitOfWork _unitOfWork,
    IConfiguration _configuration,
    IJwtTokenService _jwtTokenService,
    UserManager<User> _userManager,
    RoleManager<Role> _roleManager,
    IMapper _mapper) : IAuthenticationService
{
    public async Task<Result<AuthenticationResponseDto>> LoginAsync(
        string login,
        string password,
        string? ipAddress = null,
        string? deviceInfo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Info("Login attempt for {Login}", login);

            var user = await _userManager.FindByEmailAsync(login)
                       ?? await _userManager.FindByNameAsync(login);

            if (user == null)
            {
                _logger.Warn("Login failed for {Login}: user was not found", login);
                return _errorService.CreateFailure<AuthenticationResponseDto>(ErrorCodes.USER_INVALID_CREDENTIALS);
            }

            if (!_passwordService.VerifyPassword(password, user.PasswordHash!, cancellationToken))
            {
                _logger.Warn("Login failed for user {UserId}: invalid credentials", user.Id);
                return _errorService.CreateFailure<AuthenticationResponseDto>(ErrorCodes.USER_INVALID_CREDENTIALS);
            }

          

            var loginRoleNames = await _userManager.GetRolesAsync(user);
            var loginRoleName = loginRoleNames.FirstOrDefault();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Role, loginRoleName ?? string.Empty),
            };

            var jwtSettings = _configuration.GetSection(ConfigurationKeys.Sections.JwtSettings);
            var accessTokenExpiryMinutes = int.Parse(jwtSettings[ConfigurationKeys.JwtSettingKeys.ExpiryMinutes] ?? "60");
            var refreshTokenExpiryDays = int.Parse(jwtSettings[ConfigurationKeys.JwtSettingKeys.RefreshTokenExpiryDays] ?? "7");

            var accessToken = _jwtTokenService.GenerateAccessToken(claims, cancellationToken);
            var refreshTokenValue = _jwtTokenService.GenerateRefreshToken(cancellationToken);
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes);
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

            var refreshToken = new RefreshToken
            {
                User = user,
                Token = refreshTokenValue,
                ExpiresAt = refreshTokenExpiresAt,
                IsRevoked = false,
                IPAddress = ipAddress,
                DeviceInfo = deviceInfo,
                CreatedOn = DateTime.UtcNow
            };

            await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken, cancellationToken);

            await _unitOfWork.Repository<User>().UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.Info("User {UserId} logged in successfully", user.Id);

            var newUser = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Role = await GetRoleDtoAsync(user),
            };

            var response = new AuthenticationResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshTokenExpiresAt = refreshTokenExpiresAt,
                User = newUser,
                Role = await GetRoleDtoAsync(user),
            };

            return Result<AuthenticationResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Login failed for {Login}", login);
            return _errorService.CreateFailure<AuthenticationResponseDto>(ErrorCodes.EXTERNAL_SERVICE_ERROR);
        }
    }

    public async Task<Result<bool>> LogoutAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.Info("Logout attempt for user {UserId}", userId);

            var activeTokens = await _unitOfWork.Repository<RefreshToken>()
                .AsQueryable()
                .Where(t => t.User!.Id.ToString() == userId && !t.IsRevoked)
                .ToListAsync(cancellationToken);

            // Return not found only if the user doesn't exist at all
            var userExists = await _userManager.FindByIdAsync(userId) != null;
            if (!userExists)
            {
                _logger.Warn("Logout failed for user {UserId}: user was not found", userId);
                return _errorService.CreateFailure<bool>(ErrorCodes.USER_NOT_FOUND);
            }

            if (activeTokens.Any())
            {
                foreach (var token in activeTokens)
                {
                    token.IsRevoked = true;
                    token.RevokedAt = DateTime.UtcNow;
                    token.RevokedBy = "User logout";
                }

                await _unitOfWork.Repository<RefreshToken>().UpdateRangeAsync(activeTokens, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.Info("Revoked {RefreshTokenCount} refresh token(s) for user {UserId}", activeTokens.Count, userId);
            }
            else
                _logger.Info("No active refresh tokens found for user {UserId} during logout", userId);

            _logger.Info("User {UserId} logged out successfully", userId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Logout failed for user {UserId}", userId);
            return _errorService.CreateFailure<bool>(ErrorCodes.EXTERNAL_SERVICE_ERROR);
        }
    }

    private async Task<RoleDto?> GetRoleDtoAsync(User user)
    {
        var names = await _userManager.GetRolesAsync(user);
        var name = names.FirstOrDefault();
        if (name == null) return null;
        var role = await _roleManager.FindByNameAsync(name);
        return role != null ? _mapper.Map<RoleDto>(role) : null;
    }
}

