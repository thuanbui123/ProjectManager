using CORE.Abstractions;
using CORE.Entities;
using CORE.Models;
using CORE.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IAuthService _authService;

    public AuthController(IJwtService jwtService, IAuthService authService)
    {
        _jwtService = jwtService;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
    {
        var rs = await _authService.Login(request.Email, request.Password);
        if(rs != null && rs.IsLogin)
        {
            var accessToken = _jwtService.GenerateAccessToken(rs.UserId, rs.Username, rs.RoleName);

            var handler = new JwtSecurityTokenHandler();
            var jwtId = handler.ReadJwtToken(accessToken).Id;

            var refreshToken = await _jwtService.GenerateRefreshToken(rs.UserId, jwtId);

            return Ok(new TokenResponseModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                AccessTokenExpires = DateTime.UtcNow.AddMinutes(15)
            });
        }

        return Unauthorized();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
    {

        var rs = await _authService.RegisterUser(request.Username, request.Email, request.Password);
        return StatusCode(201, rs);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenResponseModel request)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return Unauthorized();

        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaim, out var userId);

        var jwtId = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
        var oldRefreshToken = await _jwtService.ValidateRefreshToken(request.RefreshToken);
        if (oldRefreshToken == null)
            return Unauthorized("Refresh token expired or used");

        await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken);
        var newAccessToken = _jwtService.GenerateAccessToken(userId, principal.Identity!.Name!, principal.FindFirstValue(ClaimTypes.Role)!);
        var newJwtId = new JwtSecurityTokenHandler().ReadJwtToken(newAccessToken).Id;
        var newRefreshToken = await _jwtService.GenerateRefreshToken(userId, newJwtId);

        return Ok(new TokenResponseModel
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            AccessTokenExpires = DateTime.UtcNow.AddMinutes(15)
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly() => Ok("✅ You are an Admin");
}
