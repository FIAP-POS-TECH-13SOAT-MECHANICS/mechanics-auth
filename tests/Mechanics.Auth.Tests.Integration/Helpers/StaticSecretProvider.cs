using Mechanics.Auth.Infra.SecretProvider;
using System.Security.Cryptography;
using System.Text.Json;

namespace Mechanics.Auth.Tests.Integration.Helpers;

public class StaticSecretProvider(RSA privateKey, string connectionString) : ISecretProvider
{
    public Task<RSA> GetPrivateKey() =>
        Task.FromResult(privateKey);

    public Task<string> GetDbConnectionString() =>
        Task.FromResult(JsonSerializer.Serialize(new { value = connectionString }));
}
