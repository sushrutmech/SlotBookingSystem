using System;
using System.Collections.Generic;

namespace Sport_Slot_booking_system.Api.Models;

public partial class SlotBooking
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int FacilityId { get; set; }

    public int StatusId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Facility Facility { get; set; } = null!;

    public virtual BookingStatusMaster Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
