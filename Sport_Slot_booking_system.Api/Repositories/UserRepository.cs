using Microsoft.EntityFrameworkCore;
using Sport_Slot_booking_system.Api.Data;
using Sport_Slot_booking_system.Api.Interfaces;
using Sport_Slot_booking_system.Api.Models;

namespace Sport_Slot_booking_system.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    // ✅ Get IQueryable (CORE for GRID)
    public IQueryable<User> GetQueryable()
    {
        return _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .AsNoTracking()   // 🔥 important for performance
            .AsQueryable();
    }
    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .ThenInclude(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}