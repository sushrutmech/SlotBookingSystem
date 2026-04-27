using System;
using System.Collections.Generic;
using System.Text;

namespace Sport_Slot_booking_system.Core.Model;

public class LoginDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}