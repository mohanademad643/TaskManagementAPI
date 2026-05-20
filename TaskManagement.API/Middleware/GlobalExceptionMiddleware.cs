using System.Net;
using System.Text.Json;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Wrappers;

namespace TaskManagement.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            NotFoundException ex => (HttpStatusCode.NotFound, ApiResponse.Fail(ex.Message)),
            ForbiddenException ex => (HttpStatusCode.Forbidden, ApiResponse.Fail(ex.Message)),
            ValidationException ex => (HttpStatusCode.BadRequest, ApiResponse.Fail(ex.Errors)),
            ConflictException ex => (HttpStatusCode.Conflict, ApiResponse.Fail(ex.Message)),
            _ => (HttpStatusCode.InternalServerError, ApiResponse.Fail("An unexpected error occurred."))
        };

        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
