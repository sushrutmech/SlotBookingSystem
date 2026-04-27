using System;
using System.Collections.Generic;
using System.Text;

namespace Sport_Slot_booking_system.Core.Model;

public class PaginationResponse<T> where T : class
{
    public PaginationResponse() { }
    public PaginationResponse(long totalRecords, T paginatedData)
    {
        TotalRecords = totalRecords;
        PaginatedData = paginatedData;
    }
  
    public long TotalRecords { get; set; }
    public T PaginatedData { get; set; }
}