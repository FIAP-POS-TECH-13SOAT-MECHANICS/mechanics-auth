using System.Security.Cryptography;

namespace Mechanics.Auth.Infra.SecretProvider;

public interface ISecretProvider
{
    Task<RSA> GetPrivateKey();
    Task<string> GetDbConnectionString();
}
