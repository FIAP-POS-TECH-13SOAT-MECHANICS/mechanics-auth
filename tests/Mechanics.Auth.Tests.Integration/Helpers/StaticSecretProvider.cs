using Mechanics.Auth.Infra.SecretProvider;
using System.Security.Cryptography;

namespace Mechanics.Auth.Tests.Integration.Helpers;

public class StaticSecretProvider(RSA privateKey) : ISecretProvider
{
    public Task<RSA> GetPrivateKey() =>
        Task.FromResult(privateKey);
}
