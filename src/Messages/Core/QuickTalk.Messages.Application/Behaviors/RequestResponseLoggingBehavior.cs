using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QuickTalk.Messages.Domain.Entities;

namespace QuickTalk.Messages.Application.Behaviors;

public class RequestResponseLoggingBehavior<TRequest, TResponse>(
    IHttpContextAccessor httpContext,
    ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, OperationResult<TResponse>>
    where TRequest: notnull, IRequest<OperationResult<TResponse>>
{
    public async Task<OperationResult<TResponse>> Handle(
        TRequest request,
        RequestHandlerDelegate<OperationResult<TResponse>> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Behavior class");

        if (httpContext.HttpContext == null)
        {
            logger.LogError("Current request {@Request} is null", request);
            throw new NullReferenceException();
        }

        var requestId = httpContext.HttpContext.TraceIdentifier;
        logger.LogInformation("Executing Request  {Id}: {@requestBody}", requestId, request);

        var response = await next();
        if (response.IsSuccess)
        {
            logger.LogInformation("Response for request: {@RequestId}: {@Response}",
                requestId, response);
        }
        else if (response.IsFailure)
        {
            var message = response.Error!.message;
            var error = response.Error.reason;
            logger.LogError("Error when executing response for request {@RequestId}: {@Message}; Error:{Error}",
                requestId, message, error);
        }

        return response;
    }
}
