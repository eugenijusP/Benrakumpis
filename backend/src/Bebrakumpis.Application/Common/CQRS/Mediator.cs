using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Bebrakumpis.Application.Common.CQRS;

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private static readonly ConcurrentDictionary<(Type, Type), object> _wrappers = new();

    public Task<TResult> SendAsync<TResult>(IRequest<TResult> request, CancellationToken ct)
    {
        var requestType = request.GetType();
        var wrapper = (HandlerWrapper<TResult>)_wrappers.GetOrAdd(
            (requestType, typeof(TResult)),
            static key => Activator.CreateInstance(
                typeof(HandlerWrapper<,>).MakeGenericType(key.Item1, key.Item2))!);

        return wrapper.HandleAsync(serviceProvider, request, ct);
    }

    private abstract class HandlerWrapper<TResult>
    {
        public abstract Task<TResult> HandleAsync(IServiceProvider sp, IRequest<TResult> request, CancellationToken ct);
    }

    private class HandlerWrapper<TRequest, TResult> : HandlerWrapper<TResult>
        where TRequest : IRequest<TResult>
    {
        public override Task<TResult> HandleAsync(IServiceProvider sp, IRequest<TResult> request, CancellationToken ct)
        {
            var handler = sp.GetService<IRequestHandler<TRequest, TResult>>()
                ?? throw new InvalidOperationException($"No handler registered for '{typeof(TRequest).Name}'.");

            RequestHandlerDelegate<TResult> pipeline = () => handler.HandleAsync((TRequest)request, ct);

            foreach (var behavior in sp.GetServices<IPipelineBehavior<TRequest, TResult>>().Reverse())
            {
                var next = pipeline;
                var b = behavior;
                pipeline = () => b.HandleAsync((TRequest)request, next, ct);
            }

            return pipeline();
        }
    }
}
