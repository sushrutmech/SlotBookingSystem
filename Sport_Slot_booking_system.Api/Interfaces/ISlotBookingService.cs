using Sport_Slot_booking_system.Api.DTOs;
using Sport_Slot_booking_system.Api.Helpers.Common.Models;

namespace Sport_Slot_booking_system.Api.Interfaces
{
    public interface ISlotBookingService
    {
        Task<PaginationResponse<List<SlotBookingDto>>> GetSlotBookingGridAsync(PaginationDomain request);
        Task CreateAsync(CreateBookingDto dto, Guid UserId);
        Task UpdateStatusAsync(Guid bookingId, int statusId);
    }
}
