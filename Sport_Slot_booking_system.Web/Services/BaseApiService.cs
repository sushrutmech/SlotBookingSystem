namespace Sport_Slot_booking_system.Web.Services;

using Radzen;
using Sport_Slot_booking_system.Core.Model;
using System.Net.Http.Json;

public class BaseApiService
{
    protected readonly HttpClient _http;
    protected readonly NotificationService _notification;
    protected readonly ILogger<BaseApiService> _logger;

    public BaseApiService(
        HttpClient http,
        NotificationService notification,
        ILogger<BaseApiService> logger)
    {
        _http = http;
        _notification = notification;
        _logger = logger;
    }

    // ✅ GENERIC POST
    protected async Task<T?> PostAsync<T>(string url, object data, string successMessage = "") where T : class
    {
        try
        {
            var res = await _http.PostAsJsonAsync(url, data);

            var result = await res.Content.ReadFromJsonAsync<ApiResponse<T>>();

            if (!res.IsSuccessStatusCode)
            {
                var error = result?.Errors?.FirstOrDefault() ?? "Request failed";

                ShowError(error);
                return default;
            }
            if (result == null)
            {
                ShowError("Failed to deserialize response");
                return default;
            }
            if (result?.Data == null)
            {
                ShowError("Invalid server response");
                return default;
            }
            if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ShowError("Session expired. Please login again.");
                return default;
            }
            if (!string.IsNullOrEmpty(successMessage))
                ShowSuccess(successMessage);

            return result.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API POST error");

            ShowError("Something went wrong");
            return default;
        }
    }

    // ✅ GENERIC GET
    protected async Task<T?> GetAsync<T>(string url) where T : class
    {
        try
        {
            var res = await _http.GetAsync(url);

            var result = await res.Content.ReadFromJsonAsync<ApiResponse<T>>();
            if (result == null)
            {
                ShowError("Failed to deserialize response");
                return default;
            }
            if (!res.IsSuccessStatusCode)
            {
                var error = result?.Errors?.FirstOrDefault() ?? "Request failed";

                ShowError(error);
                return default;
            }
            if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                ShowError("Session expired. Please login again.");
                return default;
            }
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API GET error");

            ShowError("Something went wrong");
            return default;
        }
    }

    // 🔔 Helpers
    protected void ShowError(string message)
    {
        _notification.Notify(NotificationSeverity.Error, "Error", message, 4000);
    }

    protected void ShowSuccess(string message)
    {
        _notification.Notify(NotificationSeverity.Success, "Success", message, 3000);
    }
}