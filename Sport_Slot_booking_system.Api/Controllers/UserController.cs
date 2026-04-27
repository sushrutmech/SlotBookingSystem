namespace Sport_Slot_booking_system.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Sport_Slot_booking_system.Api.DTOs;
    using Sport_Slot_booking_system.Api.Helpers.Common.Models;
    using Sport_Slot_booking_system.Api.Interfaces;
    using Sport_Slot_booking_system.Api.Models;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        // ✅ USER GRID
        [HttpPost("grid")]
        public async Task<IActionResult> GetUsersGrid([FromBody] UserGridRequest request)
        {
            var response = new ApiResponse<PaginationResponse<List<UserResponseDto>>>();

            try
            {
                var result = await _service.GetUsersGridAsync(request);

                response.Data = result;
                response.Message = "Users fetched successfully";
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Errors = new[] { ex.Message };
                response.Message = "Error fetching users";
            }

            return Ok(response);
        }
    }
}
