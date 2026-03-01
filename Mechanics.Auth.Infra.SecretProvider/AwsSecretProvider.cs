using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Mechanics.Auth.Infra.SecretProvider;

public class AwsSecretProvider(ILogger<AwsSecretProvider> logger, IAmazonSecretsManager client, IOptions<SecretProviderOptions> options)
    : ISecretProvider
{
    private RSA? _privateKey;

    public async Task<RSA> GetPrivateKey()
    {
        if (_privateKey is not null)
        {
            logger.LogDebug("Returning cached private key");
            return _privateKey;
        }

        _privateKey = RSA.Create();
        _privateKey.ImportFromPem(await GetValueFromSecret());

        return _privateKey;
    }

    private async Task<string> GetValueFromSecret()
    {
        var secretName = options.Value.PrivateKeySecretName;

        logger.LogInformation("Fetching private key from Secrets Manager using name '{SecretName}'", secretName);
        var response = await client.GetSecretValueAsync(new GetSecretValueRequest { SecretId = secretName });

        return string.IsNullOrWhiteSpace(response.SecretString)
            ? throw new InvalidOperationException("SecretString is empty.")
            : response.SecretString;
    }
}
