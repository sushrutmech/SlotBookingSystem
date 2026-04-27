using Sport_Slot_booking_system.Api.Models;
namespace Sport_Slot_booking_system.Api.Data;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    // ✅ EXISTING
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    // ✅ NEW
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<SlotBooking> SlotBookings => Set<SlotBooking>();
    public DbSet<BookingStatusMaster> BookingStatuses => Set<BookingStatusMaster>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // USER ROLE (MANY-TO-MANY)
        // =========================
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        // =========================
        // SLOT BOOKING RELATIONS
        // =========================
        modelBuilder.Entity<SlotBooking>()
            .HasOne(sb => sb.User)
            .WithMany(u => u.SlotBookings)
            .HasForeignKey(sb => sb.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SlotBooking>()
            .HasOne(sb => sb.Facility)
            .WithMany(f => f.SlotBookings)
            .HasForeignKey(sb => sb.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SlotBooking>()
            .HasOne(sb => sb.Status)
            .WithMany(s => s.SlotBookings)
            .HasForeignKey(sb => sb.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);

        // =========================
        // SEED ROLES
        // =========================
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "User" },
            new Role { Id = 2, Name = "Admin" },
            new Role { Id = 3, Name = "StaffPerson" }
        );

        // =========================
        // SEED FACILITIES
        // =========================
        modelBuilder.Entity<Facility>().HasData(
            new Facility { Id = 1, Name = "Cricket Turf" },
            new Facility { Id = 2, Name = "Swimming Pool" },
            new Facility { Id = 3, Name = "Cricket Coaching" }
        );

        modelBuilder.Entity<BookingStatusMaster>()
                    .ToTable("BookingStatusMaster");

        // =========================
        // SEED BOOKING STATUS
        // =========================
        modelBuilder.Entity<BookingStatusMaster>().HasData(
            new BookingStatusMaster { Id = 1, Name = "Pending" },
            new BookingStatusMaster { Id = 2, Name = "Booked" },
            new BookingStatusMaster { Id = 3, Name = "Cancelled" }
        );
    }
}