using AutoMapper;
using CORE.Abstractions;
using CORE.Entities;
using CORE.Models;
using CORE.Services.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CORE.Services.Implementations;

public class JwtService : IJwtService
{
    private readonly JwtSettingModel _jwtSettings = null!;
    private readonly IRepository<UserRefreshTokenEntity> _urfRepository = null!;
    private readonly IUnitOfWork _unitOfWork = null!;
    private readonly IMapper _mapper;

    public JwtService
        (IOptions<JwtSettingModel> jwtOption, IRepository<UserRefreshTokenEntity> urfRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _jwtSettings = jwtOption.Value;
        _urfRepository = urfRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public string GenerateAccessToken(Guid userId, string username, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshTokenModel> GenerateRefreshToken(Guid userId, string jwtId)
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var tokenString = Convert.ToBase64String(randomNumber);

        var rtModel = new RefreshTokenModel
        {
            UserId = userId,
            Token = tokenString,
            IsUsed = false,
            IsRevoked = false,
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays)
        };

        var rtEntity = _mapper.Map<UserRefreshTokenEntity>(rtModel);
        rtEntity.Id = Guid.NewGuid();
        rtEntity.DeviceInfo = string.Empty;
        rtEntity.CreatedDate = DateTime.Now;

        await _urfRepository.AddAsync(rtEntity);
        await _unitOfWork.CompleteAsync();

        return rtModel;
    }

    public async Task<RefreshTokenModel?> ValidateRefreshToken(string token)
    {
        var existRefreshToken = await _urfRepository.FindAsync(ur => ur.Token == token);
        if (existRefreshToken == null || !existRefreshToken.Any())
            return null;
        if (existRefreshToken.FirstOrDefault()!.IsUsed || existRefreshToken.FirstOrDefault()!.IsRevoked || existRefreshToken.FirstOrDefault()!.ExpiryDate <= DateTime.UtcNow)
            return null;
        var model = _mapper.Map<RefreshTokenModel>(existRefreshToken.FirstOrDefault());
        return model;
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // ⚠️ cho phép token hết hạn
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken)
                return null;

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token)
    {
        var existRefreshToken = await _urfRepository.FindAsync(ur => ur.Token == token);
        if (existRefreshToken == null || !existRefreshToken.Any())
            return false;
        var oldToken = existRefreshToken.FirstOrDefault();
        oldToken!.IsUsed = true;
        oldToken!.IsRevoked = true;
        oldToken!.RevokedDate = DateTime.UtcNow;

        _urfRepository.Update(oldToken);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}
