namespace Sport_Slot_booking_system.Api.Services
{
    using Microsoft.EntityFrameworkCore;
    using Sport_Slot_booking_system.Api.DTOs;
    using Sport_Slot_booking_system.Api.Helpers;
    using Sport_Slot_booking_system.Api.Helpers.Common.Models;
    using Sport_Slot_booking_system.Api.Interfaces;
    using Sport_Slot_booking_system.Api.Models;

    public class SlotBookingService : ISlotBookingService
    {
        private readonly ISlotBookingRepository _repo;

        public SlotBookingService(ISlotBookingRepository repo)
        {
            _repo = repo;
        }

        // ✅ GET ALL (Scheduler + Grid)
        public async Task<PaginationResponse<List<SlotBookingDto>>> GetSlotBookingGridAsync(PaginationDomain request)
        {
            // 🔄 PROJECT FIRST → now query is IQueryable<SlotBookingDto>
            var query = _repo.GetQueryable()
                .AsNoTracking()
                .Select(x => new SlotBookingDto
                {
                    Id = x.Id,
                    CustomerName = x.User.FirstName??"",
                    FacilityName = x.Facility.Name ?? "",
                    FacilityId = x.FacilityId,
                    StatusId = x.StatusId,
                    Status = x.Status.Name ?? "",
                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                });

            // 🔍 SEARCH on DTO properties (FacilityName now exists ✅)
            query = query.ApplySearch(request.Searches);

            // 🔽 SORT on DTO properties
            if (!string.IsNullOrEmpty(request.SortByColumn))
                query = query.OrderByProperty(request.SortByColumn, request.SortDesOrder);

            // 📊 TOTAL COUNT
            var totalRecords = await query.CountAsync();

            // 📄 PAGINATION
            var data = await PaginationHelper
                .ApplyPagination(query, request)
                .ToListAsync();

            return new PaginationResponse<List<SlotBookingDto>>(totalRecords, data);
        }
        // ✅ CREATE BOOKING
        public async Task CreateAsync(CreateBookingDto dto, Guid UserId)
        {
            // 🔥 Check overlap
            var isBooked = await _repo.IsSlotOverlapping(
                dto.FacilityId,
                dto.StartTime,
                dto.EndTime
            );

            if (isBooked)
            {
                throw new Exception("This slot is already booked. Please choose another time.");
            }

            var booking = new SlotBooking
            {
                Id = Guid.NewGuid(),
                FacilityId = dto.FacilityId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                UserId = UserId,
                StatusId=1,     // by default slot is pending so 1
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(booking);
            await _repo.SaveAsync();
        }

        // ✅ UPDATE STATUS
        public async Task UpdateStatusAsync(Guid bookingId, int statusId)
        {
            var booking = await _repo.GetByIdAsync(bookingId);

            if (booking == null)
                throw new Exception("Booking not found");

            booking.StatusId = statusId;

            await _repo.SaveAsync();
        }
    }
}
