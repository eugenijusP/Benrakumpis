using Bebrakumpis.Application.Common.CQRS;
using Microsoft.Extensions.Logging;

namespace Bebrakumpis.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResult>(ILogger<LoggingBehavior<TRequest, TResult>> logger)
    : IPipelineBehavior<TRequest, TResult> where TRequest : IRequest<TResult>
{
    public async Task<TResult> HandleAsync(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken ct)
    {
        logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        var result = await next();
        logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
        return result;
    }
}
