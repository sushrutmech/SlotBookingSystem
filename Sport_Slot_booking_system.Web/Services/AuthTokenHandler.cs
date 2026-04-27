using System.Net;
using System.Net.Http.Headers;

namespace Sport_Slot_booking_system.Web.Services;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;
    private readonly AuthService _authService;

    public AuthTokenHandler(TokenService tokenService, AuthService authService)
    {
        _tokenService = tokenService;
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 1️⃣ Attach current access token to every request
        var accessToken = await _tokenService.GetAccessToken();
        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await base.SendAsync(request, cancellationToken);

        // 2️⃣ If 401 → try silent refresh
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshed = await _authService.RefreshToken();

            if (refreshed)
            {
                // 3️⃣ Retry the original request with new token
                var newToken = await _tokenService.GetAccessToken();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                response = await base.SendAsync(request, cancellationToken);
            }
            else
            {
                // 4️⃣ Refresh failed → force logout
                await _authService.Logout();
            }
        }

        return response;
    }
}