namespace Bebrakumpis.Application.Common.CQRS;

public delegate Task<TResult> RequestHandlerDelegate<TResult>();
