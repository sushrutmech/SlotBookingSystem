using System.Net;
using System.Net.Http.Headers;
using Sport_Slot_booking_system.Core.Model;
using System.Net.Http.Json;

namespace Sport_Slot_booking_system.Web.Services;

public class AuthHttpMessageHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;
    private readonly IServiceProvider _serviceProvider;
    private const string BASE = "api/auth";

    public AuthHttpMessageHandler(TokenService tokenService, IServiceProvider serviceProvider)
    {
        _tokenService = tokenService;
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 1️⃣ Attach access token to every request
        var accessToken = await _tokenService.GetAccessToken();
        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        // 2️⃣ Skip refresh for auth endpoints (avoid infinite loop)
        var requestPath = request.RequestUri?.PathAndQuery ?? "";
        if (requestPath.Contains($"{BASE}/refresh") || requestPath.Contains($"{BASE}/login"))
            return response;

        // 3️⃣ If 401 → try silent token refresh
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshed = await TryRefreshTokenAsync();

            if (refreshed)
            {
                // 4️⃣ Retry original request with new access token
                var newToken = await _tokenService.GetAccessToken();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                response = await base.SendAsync(request, cancellationToken);
            }
            else
            {
                // 5️⃣ Refresh failed → force logout
                var authService = _serviceProvider.GetRequiredService<AuthService>();
                await authService.Logout();
            }
        }

        return response;
    }

    // ✅ Direct HTTP call to avoid circular dependency (no handler here)
    private async Task<bool> TryRefreshTokenAsync()
    {
        try
        {
            var refreshToken = await _tokenService.GetRefreshToken();
            if (string.IsNullOrEmpty(refreshToken)) return false;

            // Use a plain HttpClient — bypasses this handler to avoid infinite loop
            using var plainHttp = new HttpClient { BaseAddress = new Uri("https://localhost:7178/") };

            var res = await plainHttp.PostAsJsonAsync($"{BASE}/refresh", new
            {
                RefreshToken = refreshToken
            });

            if (!res.IsSuccessStatusCode) return false;

            var result = await res.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
            if (result?.Data == null) return false;

            await _tokenService.SetTokens(result.Data.AccessToken, result.Data.RefreshToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}