using DotNet.Testcontainers.Containers;
using System.Security.Cryptography;

namespace Mechanics.Auth.Tests.Integration.Helpers;

[TestClass]
public static class TestProperties
{
    public static ApplicationFactory Factory { get; private set; } = null!;
    private static IContainer _awsClientContainer = null!;

    [AssemblyInitialize]
    public static async Task Setup(TestContext context)
    {
        Environment.SetEnvironmentVariable("JwtOptions__AccessTokenLifetime", "60");
        Environment.SetEnvironmentVariable("JwtOptions__RefreshTokenSalt", "ef932c68c005c43607b2e076ced1472c");

        var rsa = RSA.Create();
        var secretProvider = new StaticSecretProvider(rsa);

        await SetupAwsClient(context);

        Factory = new ApplicationFactory(secretProvider);
    }

    private static async Task SetupAwsClient(TestContext context)
    {
        _awsClientContainer = new TestAwsClientContainer().Container;
        await _awsClientContainer.StartAsync(context.CancellationTokenSource.Token);

        Environment.SetEnvironmentVariable("AwsCredentials__UseLocalstack", "true");
        var localstackUrl = $"http://localhost:{_awsClientContainer.GetMappedPublicPort(4566)}";
        Environment.SetEnvironmentVariable("AwsCredentials__LocalstackUrl", localstackUrl);
    }

    [AssemblyCleanup]
    public static async Task Cleanup()
    {
        await Factory.DisposeAsync();
        await _awsClientContainer.DisposeAsync();
    }
}
