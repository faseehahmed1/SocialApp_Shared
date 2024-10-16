using FluentValidation;
using SocialApp.Middleware.Exceptions;

namespace SocialApp.Middleware;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context); // Proceed with the request
        }
        catch (ValidationException ex) // Handle FluentValidation errors
        {
            logger.LogError(ex, ex.Message);
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (NotFoundException ex) // Handle NotFoundException errors
        {
            logger.LogError(ex, ex.Message);
            await HandleNotFoundExceptionAsync(context, ex);
        }
        catch (Exception ex) // Handle general errors
        {
            logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });

        return context.Response.WriteAsJsonAsync(new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Validation failed",
            Errors = errors
        });
    }

    private static Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        return context.Response.WriteAsJsonAsync(new
        {
            StatusCode = context.Response.StatusCode,
            Message = ex.Message
        });
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return context.Response.WriteAsJsonAsync(new
        {
            StatusCode = context.Response.StatusCode,
            Message = "An unexpected error occurred",
            Details = ex.Message
        });
    }
}