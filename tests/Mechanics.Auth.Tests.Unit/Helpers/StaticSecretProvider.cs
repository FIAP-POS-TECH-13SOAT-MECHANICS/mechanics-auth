using Mechanics.Auth.Infra.SecretProvider;
using System.Security.Cryptography;

namespace Mechanics.Auth.Tests.Unit.Helpers;

public class StaticSecretProvider(RSA privateKey) : ISecretProvider
{
    public Task<RSA> GetPrivateKey() =>
        Task.FromResult(privateKey);

    public Task<string> GetDbConnectionString() =>
        Task.FromResult("Server=localhost;Database=fiap-mechanics;User Id=sa;Password=2%r6dZ6Xk@g3;TrustServerCertificate=True;");
}
