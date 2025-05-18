using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net;
using System.Net.Mime;

namespace ManualProg.Api.Core;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var message = exception.Message ?? "Unhandled exception occurred";

        _logger.LogError(exception, message);

        var problemDetails = new ProblemDetails
        {
            Status = (int)GetHttpStatusCode(exception),
            Instance = context.Request.Path,
            Title = message,
        };

        context.Response.ContentType = MediaTypeNames.Application.ProblemJson;
        context.Response.StatusCode = (int)problemDetails.Status;

        return context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static HttpStatusCode GetHttpStatusCode(Exception exception)
    {
        return exception switch
        {
            ReadOnlyException or UnauthorizedAccessException => HttpStatusCode.Forbidden,
            InvalidOperationException or NotSupportedException or ValidationException => HttpStatusCode.BadRequest,
            FileNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };
    }
}
