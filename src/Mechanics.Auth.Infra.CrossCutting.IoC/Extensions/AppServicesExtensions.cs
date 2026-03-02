using Mechanics.Auth.Application;
using Mechanics.Auth.Application.Options;
using Mechanics.Auth.Application.TokenGenerator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mechanics.Auth.Infra.CrossCutting.IoC.Extensions;

public static class AppServicesExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        var appServices = typeof(IAppService).Assembly.GetTypes()
            .Where(type => type.GetInterfaces().Contains(typeof(IAppService)));
        foreach (var appService in appServices)
            services.AddScoped(appService);

        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        services.AddScoped<IJwtTokenHandler, JwtTokenHandler>()
            .AddSingleton(TimeProvider.System);

        return services;
    }
}
