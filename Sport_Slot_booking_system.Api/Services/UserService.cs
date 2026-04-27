namespace Sport_Slot_booking_system.Api.Services;

using Microsoft.EntityFrameworkCore;
using Sport_Slot_booking_system.Api.DTOs;
using Sport_Slot_booking_system.Api.Helpers;
using Sport_Slot_booking_system.Api.Helpers.Common.Models;
using Sport_Slot_booking_system.Api.Interfaces;
using Sport_Slot_booking_system.Api.Models;
using System.Security.Cryptography;
using System.Text;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly JwtHelper _jwt;
    private readonly IRefreshTokenRepository _refreshRepo;

    public UserService(
       IUserRepository repo,
       JwtHelper jwt,
       IRefreshTokenRepository refreshRepo)
    {
        _repo = repo;
        _jwt = jwt;
        _refreshRepo = refreshRepo;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existing = await _repo.GetByEmailAsync(dto.Email);
        if (existing != null)
            throw new Exception("User already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            Email = dto.Email,
            MobileNo = dto.MobileNo,
            PasswordHash = Hash(dto.Password),
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            UserRoles = new List<UserRole>
        {
            new UserRole { RoleId = 1 }
        }
        };

        await _repo.AddAsync(user);

        // 🔐 TOKENS
        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();

        await SaveRefreshToken(user.Id, refreshToken);

        return new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email!,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _repo.GetByEmailAsync(dto.Email);
        if (user == null) return null;

        if (user.PasswordHash != Hash(dto.Password)) return null;

        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();

        await SaveRefreshToken(user.Id, refreshToken);

        return new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email!,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(string token)
    {
        var existing = await _refreshRepo.GetByTokenAsync(token);

        if (existing == null || existing.IsRevoked || existing.ExpiresAt < DateTime.UtcNow)
            return null;

        // 🔥 REVOKE OLD TOKEN
        existing.IsRevoked = true;

        var user = existing.User;

        var newAccessToken = _jwt.GenerateAccessToken(user);
        var newRefreshToken = _jwt.GenerateRefreshToken();

        await SaveRefreshToken(user.Id, newRefreshToken);

        await _refreshRepo.SaveAsync();

        return new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email!,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        var token = await _refreshRepo.GetByTokenAsync(refreshToken);

        if (token == null) return false;

        token.IsRevoked = true;

        await _refreshRepo.SaveAsync();

        return true;
    }

    private async Task SaveRefreshToken(Guid userId, string token)
    {
        var refresh = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshRepo.AddAsync(refresh);
        await _refreshRepo.SaveAsync();
    }

    private string Hash(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }

    public async Task<PaginationResponse<List<UserResponseDto>>> GetUsersGridAsync(UserGridRequest request)
    {
        var query = _repo.GetQueryable();

        // 🔍 SEARCH
        query = query.ApplySearch(request.Searches);

        // 🔽 SORT
        if (!string.IsNullOrEmpty(request.SortByColumn))
        {
            query = query.OrderByProperty(request.SortByColumn, request.SortDesOrder);
        }

        // 📊 TOTAL COUNT
        var totalRecords = await query.CountAsync();

        // 📄 PAGINATION
        var users = await PaginationHelper
            .ApplyPagination(query, request)
            .ToListAsync();

        // 🔄 MAP TO DTO (KEY FIX)
        var dtoList = users.Select(MapToDto).ToList();

        return new PaginationResponse<List<UserResponseDto>>(totalRecords, dtoList);
    }

    private UserResponseDto MapToDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Email = user.Email,
            MobileNo = user.MobileNo,
            DateOfBirth = user.DateOfBirth?? DateTime.MinValue,
            Gender = user.Gender,
            Roles = user.UserRoles.Select(x => x.Role.Name).ToList()
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var token = await _repo.GetRefreshTokenAsync(refreshToken);

        if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
            return false;

        token.IsRevoked = true;

        await _repo.SaveAsync();

        return true;
    }
}