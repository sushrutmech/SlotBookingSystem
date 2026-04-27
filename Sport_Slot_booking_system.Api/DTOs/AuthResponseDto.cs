namespace Sport_Slot_booking_system.Api.DTOs
{
    public class AuthResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
