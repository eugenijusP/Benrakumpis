using System.Reflection;
using Bebrakumpis.Application.Common.Behaviors;
using Bebrakumpis.Application.Common.CQRS;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bebrakumpis.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        var assembly = Assembly.GetExecutingAssembly();
        var handlerInterface = typeof(IRequestHandler<,>);
        var validatorInterface = typeof(IValidator<>);

        foreach (var type in assembly.GetTypes().Where(t => t is { IsAbstract: false, IsInterface: false }))
        {
            foreach (var iface in type.GetInterfaces().Where(i => i.IsGenericType &&
                (i.GetGenericTypeDefinition() == handlerInterface ||
                 i.GetGenericTypeDefinition() == validatorInterface)))
            {
                services.AddScoped(iface, type);
            }
        }

        return services;
    }
}
