using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using TaskBoard.Api.Exceptions;

namespace TaskBoard.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
            var statusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ForbiddenException => StatusCodes.Status403Forbidden,
                ConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            if (statusCode == 500)
                _logger.LogError(ex, "Beklenmeyen hata");

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var message = statusCode == 500
                ? "Sunucuda beklenmeyen bir hata oluştu."
                : ex.Message;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new { message }));


        }
    }
    
}