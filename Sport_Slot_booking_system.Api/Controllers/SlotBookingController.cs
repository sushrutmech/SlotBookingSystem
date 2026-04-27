namespace Sport_Slot_booking_system.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Sport_Slot_booking_system.Api.DTOs;
    using Sport_Slot_booking_system.Api.Helpers.Common.Models;
    using Sport_Slot_booking_system.Api.Interfaces;
    using System.Security.Claims;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SlotBookingController : ControllerBase
    {
        private readonly ISlotBookingService _service;

        public SlotBookingController(ISlotBookingService service)
        {
            _service = service;
        }

        [HttpPost("gridPagginated")]
        public async Task<IActionResult> GetGridPagginated([FromBody] PaginationDomain request)
        {
            var response = new ApiResponse<PaginationResponse<List<SlotBookingDto>>>();
            try
            {
                var result = await _service.GetSlotBookingGridAsync(request);
                response.Data = result;
                response.Message = "Success";
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Errors = new[] { ex.Message };
                response.Message = "Error fetching bookings";
            }
            return Ok(response);
        }

        // ✅ GET ALL BOOKINGS
        [HttpGet]
        [HttpPost("grid")]
        public async Task<ApiResponse<PaginationResponse<List<SlotBookingDto>>>> GetGrid([FromBody] PaginationDomain request)
        {
            var data = await _service.GetSlotBookingGridAsync(request);

            return new ApiResponse<PaginationResponse<List<SlotBookingDto>>>
            {
                Data = data,
                Message = "Success"
            };
        }

        // ✅ CREATE BOOKING
        [HttpPost("bookslot")]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
        {
            var response = new ApiResponse<string>();

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var userId = Guid.Parse(userIdClaim);

                await _service.CreateAsync(dto, userId);

                return Ok(new ApiResponse<string>
                {
                    Data = "Booking successful"
                });
            }
            catch (Exception ex)
            {
                response.Code = 400;
                response.Errors = new[] { ex.Message };
            }

            return StatusCode(response.Code, response);
        }

        // ✅ UPDATE STATUS (Cancel / Approve etc.)
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus(Guid bookingId, int statusId)
        {
            var response = new ApiResponse<string>();

            try
            {
                await _service.UpdateStatusAsync(bookingId, statusId);
                response.Message = "Status updated";
            }
            catch (Exception ex)
            {
                response.Code = 400;
                response.Errors = new[] { ex.Message };
            }

            return StatusCode(response.Code, response);
        }
    }
}
