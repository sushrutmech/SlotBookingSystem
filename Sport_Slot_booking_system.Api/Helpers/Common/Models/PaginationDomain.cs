namespace Sport_Slot_booking_system.Api.Helpers.Common.Models
{
    public class PaginationDomain<T> : PaginationDomain where T : class
    {
        public T? RequestParameter { get; set; }
    }
    public class PaginationDomain
    {
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortByColumn { get; set; } = ""!;
        public bool SortDesOrder { get; set; }
        public string? SearchByColumn { get; set; } = ""!;
        public string? SearchText { get; set; } = ""!;

        public List<SearchDomain>? Searches { get; set; }
    }
}
