namespace Sport_Slot_booking_system.Api.Helpers.Common.Models
{
    public class PaginationResponse<T> where T : class
    {
        public PaginationResponse(long totalRecords, T paginatedData)
        {
            TotalRecords = totalRecords;
            PaginatedData = paginatedData;
        }
        public PaginationResponse(long approverConfigGridTotalCount)
        {
            TotalRecords = 0;
            PaginatedData = (T)Activator.CreateInstance(typeof(T))!;
        }
        public long TotalRecords { get; set; }
        public T PaginatedData { get; set; }
    }
}
