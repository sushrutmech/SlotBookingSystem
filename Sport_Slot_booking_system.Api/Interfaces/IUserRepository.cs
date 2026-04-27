using Sport_Slot_booking_system.Api.Models;

namespace Sport_Slot_booking_system.Api.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    IQueryable<User> GetQueryable();
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task SaveAsync();
}