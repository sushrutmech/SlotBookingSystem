using Microsoft.EntityFrameworkCore;
using Sport_Slot_booking_system.Api.Data;
using Sport_Slot_booking_system.Api.Interfaces;
using Sport_Slot_booking_system.Api.Models;

namespace Sport_Slot_booking_system.Api.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
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