namespace Sport_Slot_booking_system.Api.Helpers.Common.Models
{
    public class ApiResponse<T> where T : class
    {
        public ApiResponse()
        {
            Errors = [];
            Code = ResponseCode.Ok;

        }
        public string? Message { get; set; }
        public int Code { get; set; }
        public string[] Errors { get; set; }
        public T? Data { get; set; }
    }
}
