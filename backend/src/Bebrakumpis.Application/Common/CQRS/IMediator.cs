namespace Bebrakumpis.Application.Common.CQRS;

public interface IMediator
{
    Task<TResult> SendAsync<TResult>(IRequest<TResult> request, CancellationToken ct);
}
