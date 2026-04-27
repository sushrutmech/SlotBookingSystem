using Sport_Slot_booking_system.Api.DTOs;
using Sport_Slot_booking_system.Api.Helpers.Common.Models;
using Sport_Slot_booking_system.Api.Models;

namespace Sport_Slot_booking_system.Api.Interfaces;

public interface IUserService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<PaginationResponse<List<UserResponseDto>>> GetUsersGridAsync(UserGridRequest request);
    Task<AuthResponseDto?> RefreshTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string refreshToken);
}