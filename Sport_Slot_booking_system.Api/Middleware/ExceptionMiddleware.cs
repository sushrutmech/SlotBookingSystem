
using Sport_Slot_booking_system.Api.Helpers.Common.Models;
using System.Net;
using System.Text.Json;

namespace Sport_Slot_booking_system.Api.Middleware;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // ✅ FULL EXCEPTION LOG
            _logger.LogError(ex,
                "Unhandled Exception | Path: {Path} | Method: {Method} | Query: {QueryString}",
                context.Request.Path,
                context.Request.Method,
                context.Request.QueryString
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ApiResponse<object>
            {
                Message = "Internal Server Error",
                Code = 500,
                Errors = new[] { "Something went wrong. Please try again." }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}