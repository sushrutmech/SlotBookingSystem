using System;
using System.Collections.Generic;
using System.Text;

namespace Sport_Slot_booking_system.Core.Model;
public class ApiResponse<T> where T : class
{
    public ApiResponse()
    {
        Errors = Array.Empty<string>();
        Code = 200;
    }

    public string? Message { get; set; }

    public int Code { get; set; }

    public string[] Errors { get; set; }

    public T? Data { get; set; }
}