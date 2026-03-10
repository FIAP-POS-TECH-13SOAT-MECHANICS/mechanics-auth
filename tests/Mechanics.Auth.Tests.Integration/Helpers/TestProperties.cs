using System.Security.Cryptography;
using Testcontainers.MsSql;

namespace Mechanics.Auth.Tests.Integration.Helpers;

[TestClass]
public static class TestProperties
{
    public static ApplicationFactory Factory { get; private set; } = null!;
    private static MsSqlContainer _msSqlContainer = null!;

    [AssemblyInitialize]
    public static async Task Setup(TestContext context)
    {
        Environment.SetEnvironmentVariable("JwtOptions__AccessTokenLifetime", "60");

        var rsa = RSA.Create();
        var connectionString = await CreateDatabaseContainer(context.CancellationTokenSource.Token);
        var secretProvider = new StaticSecretProvider(rsa, connectionString);

        Factory = new ApplicationFactory(secretProvider);
    }

    private static async Task<string> CreateDatabaseContainer(CancellationToken cancellationToken)
    {
        var testDatabaseContainer = new TestDatabaseContainer();
        _msSqlContainer = testDatabaseContainer.Container;
        await _msSqlContainer.StartAsync(cancellationToken);

        await testDatabaseContainer.SeedDatabase(cancellationToken);

        return _msSqlContainer.GetConnectionString();
    }

    [AssemblyCleanup]
    public static async Task Cleanup()
    {
        await Factory.DisposeAsync();
        await _msSqlContainer.DisposeAsync();
    }
}
