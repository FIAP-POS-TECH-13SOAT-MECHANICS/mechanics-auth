using Mechanics.Auth.Infra.Data.CachedRepository;
using Mechanics.Auth.Infra.Data.Connection;
using Mechanics.Auth.Infra.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mechanics.Auth.Infra.CrossCutting.IoC.Extensions;

public static class DataExtension
{
    public static IServiceCollection AddDataRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, SqlServerDbConnectionFactory>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IRolesCachedRepository, RolesCachedRepository>();

        return services;
    }
}
