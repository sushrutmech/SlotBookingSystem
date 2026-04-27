using Microsoft.Extensions.Logging;
using Radzen;
using Sport_Slot_booking_system.Core.Model;

namespace Sport_Slot_booking_system.Web.Services;

public class UserService : BaseApiService
{
    private const string BASE = "api/user";

    public UserService(
        HttpClient http,
        NotificationService notification,
        ILogger<UserService> logger)
        : base(http, notification, logger)
    {
    }

    public async Task<PaginationResponse<List<UserResponseDto>>?> GetUsersGridAsync(UserGridRequest request)
    {
        return await PostAsync<PaginationResponse<List<UserResponseDto>>>($"{BASE}/grid", request);
    }
}