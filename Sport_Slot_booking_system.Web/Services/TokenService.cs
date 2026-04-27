namespace Sport_Slot_booking_system.Web.Services;

using Microsoft.JSInterop;

public class TokenService
{
    private readonly IJSRuntime _js;

    public TokenService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task SetTokens(string accessToken, string refreshToken)
    {
        await _js.InvokeVoidAsync("storage.set", "accessToken", accessToken);
        await _js.InvokeVoidAsync("storage.set", "refreshToken", refreshToken);
    }

    public async Task<string?> GetAccessToken()
    {
        return await _js.InvokeAsync<string>("storage.get", "accessToken");
    }

    public async Task<string?> GetRefreshToken()
    {
        return await _js.InvokeAsync<string>("storage.get", "refreshToken");
    }

    public async Task ClearTokens()
    {
        await _js.InvokeVoidAsync("storage.remove", "accessToken");
        await _js.InvokeVoidAsync("storage.remove", "refreshToken");
    }
}