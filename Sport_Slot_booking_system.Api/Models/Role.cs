using System;
using System.Collections.Generic;

namespace Sport_Slot_booking_system.Api.Models;

public partial class Role
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
