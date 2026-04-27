using System;
using System.Collections.Generic;
using System.Text;

namespace Sport_Slot_booking_system.Core.Model;

public class AuthResponseDto
{
    public Guid Id { get; set; }

    public string? Email { get; set; }

    // 🔐 Access Token (JWT)
    public string AccessToken { get; set; } = "";

    // 🔁 Refresh Token
    public string RefreshToken { get; set; } = "";
}