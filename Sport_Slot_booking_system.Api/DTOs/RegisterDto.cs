namespace Sport_Slot_booking_system.Api.DTOs;
public class RegisterDto
{
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";
    public string MobileNo { get; set; } = "";
    public string Password { get; set; } = "";

    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = "";
}