using Sport_Slot_booking_system.Api.Models;

namespace Sport_Slot_booking_system.Api.Interfaces
{
    public interface ISlotBookingRepository
    {
        IQueryable<SlotBooking> GetQueryable();
        Task AddAsync(SlotBooking booking);
        Task<SlotBooking?> GetByIdAsync(Guid id);
        Task SaveAsync();
        Task<bool> IsSlotOverlapping(int facilityId, DateTime start, DateTime end);
    }
}
