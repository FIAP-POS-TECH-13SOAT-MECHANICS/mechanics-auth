using Mechanics.Auth.Application.Consumers;
using Microsoft.Extensions.DependencyInjection;

namespace Mechanics.Auth.Infra.CrossCutting.IoC.Extensions;

public static class EventConsumersExtensions
{
    public static IServiceCollection AddEventConsumers(this IServiceCollection services)
    {
        var appServices = typeof(IEventConsumer).Assembly.GetTypes()
            .Where(type => type.GetInterfaces().Contains(typeof(IEventConsumer)));
        foreach (var appService in appServices)
            services.AddScoped(appService);

        return services;
    }
}
