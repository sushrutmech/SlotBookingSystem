using Sport_Slot_booking_system.Api.Helpers.Common.Models;

namespace Sport_Slot_booking_system.Api.Helpers
{
    public static class PaginationHelper
    {
        public static IQueryable<T> ApplyPagination<T>(IQueryable<T> query, PaginationDomain paginationRequestDto)
        {
            return query.Skip((paginationRequestDto.PageNo - 1) * paginationRequestDto.PageSize).Take(paginationRequestDto.PageSize);
        }

        public static IEnumerable<T> ApplyPagination<T>(IEnumerable<T> query, PaginationDomain paginationRequestDto)
        {
            return query.Skip((paginationRequestDto.PageNo - 1) * paginationRequestDto.PageSize).Take(paginationRequestDto.PageSize);
        }

        public static IQueryable<T> ApplyPagination<T>(IQueryable<T> query, PaginationRequest paginationRequestDto)
        {
            return query.Skip((paginationRequestDto.PageNo - 1) * paginationRequestDto.PageSize).Take(paginationRequestDto.PageSize);
        }

        public static IEnumerable<T> ApplyPagination<T>(IEnumerable<T> query, PaginationRequest paginationRequestDto)
        {
            return query.Skip((paginationRequestDto.PageNo - 1) * paginationRequestDto.PageSize).Take(paginationRequestDto.PageSize);
        }
    }

}
