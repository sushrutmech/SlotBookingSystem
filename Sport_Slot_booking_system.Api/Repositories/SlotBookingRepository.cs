namespace Sport_Slot_booking_system.Api.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Sport_Slot_booking_system.Api.Data;
    using Sport_Slot_booking_system.Api.Interfaces;
    using Sport_Slot_booking_system.Api.Models;

    public class SlotBookingRepository : ISlotBookingRepository
    {
        private readonly AppDbContext _context;

        public SlotBookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<SlotBooking> GetQueryable()
        {
            return _context.SlotBookings
                .Include(x => x.User)
                .Include(x => x.Facility)
                .Include(x => x.Status) 
                .AsQueryable();
        }


        public async Task AddAsync(SlotBooking booking)
        {
            await _context.SlotBookings.AddAsync(booking);
        }

        public async Task<SlotBooking?> GetByIdAsync(Guid id)
        {
            return await _context.SlotBookings
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsSlotOverlapping(int facilityId, DateTime start, DateTime end)
        {
            return await _context.SlotBookings
                .AnyAsync(x =>
                    x.FacilityId == facilityId &&
                    x.StartTime < end &&
                    x.EndTime > start
                );
        }
    }
}
