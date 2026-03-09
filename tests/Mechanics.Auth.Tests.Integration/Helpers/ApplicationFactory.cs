using Mechanics.Auth.Api;
using Mechanics.Auth.Infra.SecretProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mechanics.Auth.Tests.Integration.Helpers;

public class ApplicationFactory(ISecretProvider secretProvider) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(secretProvider);
        });
    }
}
