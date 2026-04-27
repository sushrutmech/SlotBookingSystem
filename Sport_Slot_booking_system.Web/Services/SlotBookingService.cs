namespace Sport_Slot_booking_system.Web.Services;

using Microsoft.Extensions.Logging;
using Radzen;
using Sport_Slot_booking_system.Core.Model;

public class SlotBookingService : BaseApiService
{
    private const string BASE = "api/slotbooking";

    public SlotBookingService(
        HttpClient http,
        NotificationService notification,
        ILogger<SlotBookingService> logger)
        : base(http, notification, logger)
    {
    }

    public async Task<bool> CreateBooking(object dto)
    {
        var result = await PostAsync<string>(
            $"{BASE}/bookslot",
            dto,
            "Booking successful"
        );

        return result != null;
    }

    public async Task<List<BookingDto>?> GetMyBookings()
    {
        var result = await PostAsync<PaginationResponse<List<BookingDto>>>(
            "api/slotbooking/grid",
            new
            {
                pageNumber = 1,
                pageSize = 50
            }
        );

        return result?.PaginatedData;
    }

    public async Task<PaginationResponse<List<BookingDto>>?> GetMyBookingsGridAsync(PaginationDomain request)
    {
        return await PostAsync<PaginationResponse<List<BookingDto>>>("api/slotbooking/gridPagginated", request);
    }

    public async Task<bool> CancelBooking(Guid id)
    {
        var res = await _http.PutAsync($"api/slotbooking/status?bookingId={id}&statusId=3", null);

        if (!res.IsSuccessStatusCode)
        {
            ShowError("Failed to cancel booking");
            return false;
        }

        ShowSuccess("Booking cancelled");
        return true;
    }
}