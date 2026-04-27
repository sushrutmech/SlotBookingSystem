using System;
using System.Collections.Generic;

namespace Sport_Slot_booking_system.Api.Models;

public partial class BookingStatusMaster
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<SlotBooking> SlotBookings { get; set; } = new List<SlotBooking>();
}
