using System;
using System.Collections.Generic;
using System.Text;

namespace Sport_Slot_booking_system.Core.Model;

public class BookingDto
{
    public Guid Id { get; set; }

    public string CustomerName { get; set; }
    public string FacilityName { get; set; }
    public int FacilityId { get; set; }

    public int StatusId { get; set; }
    public string Status { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}