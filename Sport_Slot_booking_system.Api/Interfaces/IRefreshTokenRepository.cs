using Sport_Slot_booking_system.Api.Models;

namespace Sport_Slot_booking_system.Api.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task SaveAsync();
}
