using System.ComponentModel.DataAnnotations;

namespace Sport_Slot_booking_system.Api.Helpers.Common.Models
{
    public class PaginationRequest<T> : PaginationRequest where T : class
    {
        public T? RequestParameter { get; set; }

    }

    public class PaginationRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page no. is required")]
        public int PageNo { get; set; }

        [Range(10, int.MaxValue, ErrorMessage = "Page size is required")]
        public int PageSize { get; set; }
        public string? SortByColumn { get; set; }
        public bool SortDesOrder { get; set; }
        public string? SearchByColumn { get; set; }
        public string? SearchText { get; set; }
        public List<SearchDomain> Searches { get; set; } = new List<SearchDomain>();

    }
}
