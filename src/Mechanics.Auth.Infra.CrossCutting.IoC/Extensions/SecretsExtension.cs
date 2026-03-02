using Amazon.SecretsManager;
using Mechanics.Auth.Infra.SecretProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mechanics.Auth.Infra.CrossCutting.IoC.Extensions;

public static class SecretsExtension
{
    public static IServiceCollection AddSecretProvider(this IServiceCollection services, IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection(nameof(SecretProviderOptions));
        services.Configure<SecretProviderOptions>(configurationSection);
        var options = configurationSection.Get<SecretProviderOptions>();
        if (options is null)
            throw new InvalidOperationException("Secret provider options are not set.");

        services.AddSingleton<IAmazonSecretsManager, AmazonSecretsManagerClient>();
        services.AddSingleton<ISecretProvider, AwsSecretProvider>();

        return services;
    }
}
