using System;
using System.Collections.Generic;
using System.Text;

namespace Sport_Slot_booking_system.Core.Model;
public class UserResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MobileNo { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }

    public List<string> Roles { get; set; } = new();
}