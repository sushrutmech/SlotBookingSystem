using Microsoft.AspNetCore.Mvc;
using Sport_Slot_booking_system.Api.DTOs;
using Sport_Slot_booking_system.Api.Helpers.Common.Models;
using Sport_Slot_booking_system.Api.Interfaces;

namespace Sport_Slot_booking_system.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService service, ILogger<AuthController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ============================
    // ✅ REGISTER
    // ============================
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var response = new ApiResponse<AuthResponseDto>();

        try
        {
            _logger.LogInformation("Register attempt for Email: {Email}", dto.Email);

            var result = await _service.RegisterAsync(dto);

            response.Data = result;
            response.Message = "User registered successfully";

            _logger.LogInformation("User registered successfully: {Email}", dto.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed for Email: {Email}", dto.Email);

            response.Code = 500;
            response.Errors = new[] { ex.Message };
            response.Message = "Registration failed";
        }

        return StatusCode(response.Code, response);
    }

    // ============================
    // ✅ LOGIN (ACCESS + REFRESH TOKEN)
    // ============================
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var response = new ApiResponse<AuthResponseDto>();

        try
        {
            _logger.LogInformation("Login attempt for Email: {Email}", dto.Email);

            var result = await _service.LoginAsync(dto);

            if (result == null)
            {
                _logger.LogWarning("Invalid login attempt for Email: {Email}", dto.Email);

                response.Code = 401;
                response.Errors = new[] { "Invalid email or password" };
                response.Message = "Login failed";
                return StatusCode(response.Code, response);
            }

            response.Data = result;
            response.Message = "Login successful";

            _logger.LogInformation("Login success for Email: {Email}", dto.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for Email: {Email}", dto.Email);

            response.Code = 500;
            response.Errors = new[] { ex.Message };
            response.Message = "Something went wrong";
        }

        return StatusCode(response.Code, response);
    }

    // ============================
    // ✅ REFRESH TOKEN
    // ============================
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        var response = new ApiResponse<AuthResponseDto>();

        try
        {
            _logger.LogInformation("Refresh token attempt");

            var result = await _service.RefreshTokenAsync(dto.RefreshToken);

            if (result == null)
            {
                _logger.LogWarning("Invalid refresh token used");

                response.Code = 401;
                response.Errors = new[] { "Invalid or expired refresh token" };
                response.Message = "Refresh failed";
                return StatusCode(response.Code, response);
            }

            response.Data = result;
            response.Message = "Token refreshed successfully";

            _logger.LogInformation("Token refreshed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");

            response.Code = 500;
            response.Errors = new[] { ex.Message };
            response.Message = "Something went wrong";
        }

        return StatusCode(response.Code, response);
    }

    // ============================
    // ✅ LOGOUT / REVOKE TOKEN
    // ============================
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequestDto dto)
    {
        var response = new ApiResponse<BoolResponse>();

        try
        {
            _logger.LogInformation("Revoke token attempt");

            var result = await _service.RevokeTokenAsync(dto.RefreshToken);

            if (!result)
            {
                _logger.LogWarning("Invalid revoke attempt");

                response.Code = 400;
                response.Errors = new[] { "Invalid token" };
                response.Message = "Revoke failed";

                return StatusCode(response.Code, response);
            }

            // ✅ FIX HERE
            response.Data = new BoolResponse { Value = true };
            response.Message = "Logged out successfully";

            _logger.LogInformation("Token revoked successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during revoke");

            response.Code = 500;
            response.Errors = new[] { ex.Message };
            response.Message = "Something went wrong";
        }

        return StatusCode(response.Code, response);
    }
}