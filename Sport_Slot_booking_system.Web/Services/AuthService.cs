using Microsoft.Extensions.Logging;
using Radzen;
using Sport_Slot_booking_system.Core.Model;
using Sport_Slot_booking_system.Web.Models;

namespace Sport_Slot_booking_system.Web.Services;

public class AuthService : BaseApiService
{
    private readonly TokenService _tokenService;
    private const string BASE = "api/auth";

    public AuthService(
        HttpClient http,
        TokenService tokenService,
        NotificationService notification,
        ILogger<AuthService> logger)
        : base(http, notification, logger)
    {
        _tokenService = tokenService;
    }

    public async Task<bool> Register(RegisterDto dto)
    {
        var result = await PostAsync<AuthResponseDto>($"{BASE}/register", dto, "Registered successfully!");
        return result != null;
    }

    public async Task<bool> Login(LoginDto dto)
    {
        var result = await PostAsync<AuthResponseDto>($"{BASE}/login", dto);
        if (result == null)
            return false;

        await _tokenService.SetTokens(result.AccessToken, result.RefreshToken);
        return true;
    }

    // ℹ️ This is only called manually (e.g. from a button)
    // Silent refresh is handled automatically by AuthHttpMessageHandler
    public async Task<bool> RefreshToken()
    {
        var refreshToken = await _tokenService.GetRefreshToken();
        if (string.IsNullOrEmpty(refreshToken)) return false;

        var result = await PostAsync<AuthResponseDto>($"{BASE}/refresh", new
        {
            RefreshToken = refreshToken
        });

        if (result == null) return false;

        await _tokenService.SetTokens(result.AccessToken, result.RefreshToken);
        return true;
    }

    public async Task Logout()
    {
        try
        {
            var refreshToken = await _tokenService.GetRefreshToken();
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await PostAsync<object>($"{BASE}/revoke", new
                {
                    RefreshToken = refreshToken
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout error");
        }
        finally
        {
            // ✅ Always clear tokens even if revoke fails
            await _tokenService.ClearTokens();
        }
    }
}