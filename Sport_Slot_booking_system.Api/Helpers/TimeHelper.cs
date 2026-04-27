namespace Sport_Slot_booking_system.Api.Helpers
{
    public static class TimeHelper
    {
        private static readonly TimeZoneInfo IndiaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public static DateTime ToUtc(DateTime indiaTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(indiaTime, IndiaTimeZone);
        }

        public static DateTime ToIndiaTime(DateTime utcTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, IndiaTimeZone);
        }
    }
}
