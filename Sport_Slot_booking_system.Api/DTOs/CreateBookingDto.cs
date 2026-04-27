using System.ComponentModel.DataAnnotations;

namespace Sport_Slot_booking_system.Api.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public int FacilityId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
    }
}
