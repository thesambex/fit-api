using System.Text.Json;
using FitApi.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FitApi.Api.Middlewares;

public class GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var (statusCode, title, type, detail) = exception switch
        {
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Not Found",
                "https://httpstatuses.com/404",
                exception.Message
            ),

            PaginationException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                "https://httpstatuses.com/400",
                exception.Message
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "https://httpstatuses.com/500",
                "Internal server error"
            )
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogCritical(exception, "Internal server error: {message}", exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode,
            Type = type,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json; charset=utf-8";

        var json = JsonSerializer.Serialize(problemDetails);
        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }
}