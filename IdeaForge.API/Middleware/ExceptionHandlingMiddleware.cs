using FluentValidation;
using IdeaForge.API.Common;
using System.Net;
using System.Text.Json;

namespace IdeaForge.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await WriteErrorAsync(context, ex);
        }
    }

    private static Task WriteErrorAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            ValidationException vex => (
                HttpStatusCode.BadRequest,
                string.Join("; ", vex.Errors.Select(e => e.ErrorMessage))),
            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                ex.Message),
            InvalidOperationException => (
                HttpStatusCode.BadRequest,
                ex.Message),
            _ => (
                HttpStatusCode.InternalServerError,
                "Something went wrong. Please try again.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(message);

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(json);
    }
}
