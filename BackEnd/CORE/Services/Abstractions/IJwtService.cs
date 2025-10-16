using CORE.Models;
using System.Security.Claims;

namespace CORE.Services.Abstractions;

public interface IJwtService
{
    public string GenerateAccessToken(Guid userId, string username, string role);
    public Task<RefreshTokenModel> GenerateRefreshToken(Guid userId, string jwtId);
    public Task<RefreshTokenModel?> ValidateRefreshToken(string token);
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    public Task<bool> RevokeRefreshTokenAsync(string token);
}
