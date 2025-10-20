using CORE.Abstractions;
using CORE.Entities;
using CORE.Models;
using CORE.Services.Abstractions;
using Hangfire;
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
    private readonly IBackgroundJobClient _jobClient;

    public AuthController(IJwtService jwtService, IAuthService authService, IBackgroundJobClient jobClient)
    {
        _jwtService = jwtService;
        _authService = authService;
        _jobClient = jobClient;
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
        var confirmationToken = Guid.NewGuid().ToString();
        var rs = await _authService.RegisterUser(request.Username, request.Email, request.Password, confirmationToken);
        var userId = rs.Item2 != null ? rs.Item2.ToString() : "";
        var confirmLink = GenerateLinkConfirm(userId, confirmationToken);
        _jobClient.Enqueue<IEmailService>(
            // Sử dụng Lambda để chỉ định phương thức cần gọi.
            // Hangfire sẽ tự động tìm IEmailService và gọi phương thức SendWelcomeEmailAsync.
            service => service.SendWelcomeEmailAsync(
                request.Email,
                $"Chào mừng {request.Username}! Bắt đầu Quản lý Dự án của bạn ngay.",
                request.Username,
                confirmLink
            )
        );
        return StatusCode(201, rs.Item1);
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

    private string GenerateLinkConfirm(string? userId, string confirmationToken)
    {
        var host = Request.Host.ToUriComponent();
        var confirmationLink = Url.Action(
            "ConfirmEmail",
            "Auth",
            new { userId = userId, token = confirmationToken },
            Request.Scheme,
            host
        );
        return confirmationLink ?? string.Empty;
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        var rs = await _authService.ConfirmRegister(userId, token);
        if (rs.ResultCode != 1) return BadRequest(rs.Message);
        return Ok(rs.Message);
    }

    //[Authorize(Roles = "Admin")]
    //[HttpGet("test-send-email")]
    //public async Task<IActionResult> TestSendEmail()
    //{
    //    await _emailService.SendWelcomeEmailAsync("Thuanbui18822@gmail.com", "Test", "Thuanbui", "abc");
    //    return Ok(new
    //    {
    //        Message = "Đang xử lý gửi email"
    //    });
    //}
}
