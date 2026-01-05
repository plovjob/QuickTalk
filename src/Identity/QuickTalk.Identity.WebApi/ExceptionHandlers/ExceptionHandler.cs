using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace QuickTalk.Identity.WebApi.ExceptionHandlers;

public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var problem = new ProblemDetails()
        {
            Status = context.Response.StatusCode,
            Title = "Internal Server Error",
            Detail = exception.Message,
            Instance = context.Request.Path,
            Type = "ServerError"
        };

        logger.LogError($"An exception was received: {exception.Message}");

        problem.Extensions["traceId"] = context.TraceIdentifier;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
