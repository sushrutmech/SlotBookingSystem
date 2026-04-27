// Web/Helpers/TimeHelper.cs
namespace Sport_Slot_booking_system.Web.Helpers;

public static class TimeHelper
{
    public static DateTime ToUtc(DateTime localTime)
    {
        return localTime.ToUniversalTime();
    }

    public static DateTime ToLocal(DateTime utcTime)
    {
        return utcTime.ToLocalTime();
    }
}